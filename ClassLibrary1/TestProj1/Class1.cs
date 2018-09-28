using ClassLibrary1;
using Moq;
using NUnit.Framework;
using System.Linq;
using System.Net.Mail;

namespace TestProj1
{
    [TestFixture]
    public class Class1
    {
        /// <summary>
        /// Stabs
        /// </summary>
        [Test]
        public void GetCurrentDirrectory()
        {
            // Mock.Of возвращает саму зависимость (прокси-объект), а не мок-объект.
            // Следующий код означает, что при вызове GetCurrentDirectory()
            // мы получим "D:\\Temp"
            ILoggerDependency loggerDependency =
                Mock.Of<ILoggerDependency>(d => d.GetCurrentDirectory() == "D:\\Temp");
            var currentDirectory = loggerDependency.GetCurrentDirectory();

            Assert.That(currentDirectory, Is.EqualTo("D:\\Temp"));
        }

        [Test]
        public void GetDirectoryByLoggerName()
        {
            //// Для любого аргумента метода GetDirectoryByLoggerName вернуть "C:\\Foo".
            //ILoggerDependency loggerDependency = Mock.Of<ILoggerDependency>(
            //    ld => ld.GetDirectoryByLoggerName(It.IsAny<string>()) == "C:\\Foo");

            //string directory = loggerDependency.GetDirectoryByLoggerName("anything");

            //Assert.That(directory, Is.EqualTo("C:\\Foo"));

            //-----------------------------------------------------------------------------------

            //// Инициализируем заглушку таким образом, чтобы возвращаемое значение
            //// метода GetDirrectoryByLoggerName зависело от аргумента метода.
            //// Код аналогичен заглушке вида:
            //// public string GetDirectoryByLoggername(string s) { return "C:\\" + s; }
            //Mock<ILoggerDependency> stub = new Mock<ILoggerDependency>();

            //stub.Setup(ld => ld.GetDirectoryByLoggerName(It.IsAny<string>()))
            //    .Returns<string>(name => "C:\\" + name);

            //string loggerName = "SomeLogger";
            //ILoggerDependency logger = stub.Object;
            //string directory = logger.GetDirectoryByLoggerName(loggerName);

            //Assert.That(directory, Is.EqualTo("C:\\" + loggerName));

            //-----------------------------------------------------------------------------------

            //// Свойство DefaultLogger нашей заглушки будет возвращать указанное значение
            //ILoggerDependency logger = Mock.Of<ILoggerDependency>(
            //    d => d.DefaultLogger == "DefaultLogger");

            //string defaultLogger = logger.DefaultLogger;

            //Assert.That(defaultLogger, Is.EqualTo("DefaultLogger"));

            //-----------------------------------------------------------------------------------

            //// Объединяем заглушки разных методов с помощью логического «И»
            //ILoggerDependency logger =
            //    Mock.Of<ILoggerDependency>(
            //        d => d.GetCurrentDirectory() == "D:\\Temp" &&
            //             d.DefaultLogger == "DefaultLogger" &&
            //             d.GetDirectoryByLoggerName(It.IsAny<string>()) == "C:\\Temp");

            //Assert.That(logger.GetCurrentDirectory(), Is.EqualTo("D:\\Temp"));
            //Assert.That(logger.DefaultLogger, Is.EqualTo("DefaultLogger"));
            //Assert.That(logger.GetDirectoryByLoggerName("CustomLogger"), Is.EqualTo("C:\\Temp"));

            //-----------------------------------------------------------------------------------

            var stub = new Mock<ILoggerDependency>();
            stub.Setup(ld => ld.GetCurrentDirectory()).Returns("D:\\Temp");
            stub.Setup(ld => ld.GetDirectoryByLoggerName(It.IsAny<string>())).Returns("C:\\Temp");
            stub.SetupGet(ld => ld.DefaultLogger).Returns("DefaultLogger");

            ILoggerDependency logger = stub.Object;

            Assert.That(logger.GetCurrentDirectory(), Is.EqualTo("D:\\Temp"));
            Assert.That(logger.DefaultLogger, Is.EqualTo("DefaultLogger"));
            Assert.That(logger.GetDirectoryByLoggerName("CustomLogger"), Is.EqualTo("C:\\Temp"));
        }

        ///
        /// Mocks
        ///

        public interface ILogWriter
        {
            string GetLogger();
            void SetLogger(string logger);
            void Write(string message);
        }
        public class Logger
        {
            private readonly ILogWriter _logWriter;

            public Logger(ILogWriter logWriter)
            {
                _logWriter = logWriter;
            }

            public void WriteLine(string message)
            {
                _logWriter.Write(message);
            }
        }

        [Test]
        public void TestIsWriteWasCalled()
        {
            var mock = new Mock<ILogWriter>();
            var logger = new Logger(mock.Object);

            logger.WriteLine("Hello, logger!");

            // Проверяем, что вызвался метод Write нашего мока с любым аргументом
            mock.Verify(lw => lw.Write(It.IsAny<string>()));

            //Проверка вызова метода ILogWriter.Write с заданным аргументами
            mock.Verify(lw => lw.Write("Hello, logger!"));

            //Проверка того, что метод ILogWriter.Write вызвался в точности один раз (ни больше, ни меньше)
            mock.Verify(lw => lw.Write(It.IsAny<string>()),
                Times.Once());
        }

        [Test]
        public void TestSeveralThhinks()
        {
            //var mock = new Mock<ILogWriter>();
            //mock.Setup(lw => lw.Write(It.IsAny<string>()));

            //var logger = new Logger(mock.Object);
            //logger.WriteLine("Hello, logger!");

            //// Мы не передаем методу Verify никаких дополнительных параметров.
            //// Это значит, что будут использоваться ожидания установленные
            //// с помощью mock.Setup
            //mock.Verify();

            //----------------------------------------------

            var mock = new Mock<ILogWriter>();
            mock.Setup(lw => lw.Write(It.IsAny<string>()));
            mock.Setup(lw => lw.SetLogger(It.IsAny<string>()));

            var logger = new Logger(mock.Object);
            logger.WriteLine("Hello, logger!");

            mock.Verify();
        }

        /// <summary>
        /// MockRepository
        /// </summary>

        // Использование MockRepository.Of для создания стабов. 
        [Test]
        public void TestSeveralWithMockRepository()
        {
            var repository = new MockRepository(MockBehavior.Default);
            ILoggerDependency logger = repository.Of<ILoggerDependency>()
                .Where(ld => ld.DefaultLogger == "DefaultLogger")
                .Where(ld => ld.GetCurrentDirectory() == "D:\\Temp")
                .Where(ld => ld.GetDirectoryByLoggerName(It.IsAny<string>()) == "C:\\Temp")
                .First();

            Assert.That(logger.GetCurrentDirectory(), Is.EqualTo("D:\\Temp"));
            Assert.That(logger.DefaultLogger, Is.EqualTo("DefaultLogger"));
            Assert.That(logger.GetDirectoryByLoggerName("CustomLogger"), Is.EqualTo("C:\\Temp"));
        }

        public interface ILogMailer
        {
            void Send(MailMessage message);
        }

        public class SmartLogger
        {
            public SmartLogger(ILogWriter writer, ILogMailer mailer )
            {

            }

            public void WriteLine(string message)
            {

            }
        }
        //Использование MockRepository для задания поведения нескольких мок-объектов. 
        [Test]
        public void TestSeveralWithMockRepositoryMoqs()
        {
            var repo = new MockRepository(MockBehavior.Default);
            var logWriterMock = repo.Create<ILogWriter>();
            logWriterMock.Setup(lw => lw.Write(It.IsAny<string>()));

            var logMailerMock = repo.Create<ILogMailer>();
            logMailerMock.Setup(lm => lm.Send(It.IsAny<MailMessage>()));

            var smartLogger = new SmartLogger(logWriterMock.Object, logMailerMock.Object);

            smartLogger.WriteLine("Hello, Logger");

            repo.Verify();
        }


    }
}
