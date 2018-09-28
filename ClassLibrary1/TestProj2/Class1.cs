using System;
using NUnit.Framework.Internal;
using ClassLibrary2;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace TestProj2
{
    // Стаб возвращающее указанное имя последнего пользователя
    internal class LastUsernameProviderStub : ILastUsernameProvider
    {
        // Добавляем публичное поле, для простоты тестирования и 
        // возможности повторного использования этого класса
        public string UserName;

        // Реализация метода очень простая - просто возвращаем UserName
        public string ReadLastUserName()
        {
            return UserName;
        }

        // Этот метод в данном случае вообще не интересен
        public void SaveLastUserName(string userName) { }
    }

    internal class LastUsernameProviderMock : ILastUsernameProvider
    {
        // Теперь в этом поле будет сохранятся имя последнего сохраненного пользователя
        public string SavedUserName;

        // Нам все еще нужно вернуть правильное значение из этого метода,
        // так что наш "мок" также является и "стабом"
        public string ReadLastUserName() { return "Jonh Skeet"; }

        // А вот в этом методе мы сохраним параметр в SavedUserName для 
        public void SaveLastUserName(string userName)
        {
            SavedUserName = userName;
        }
    }

    [TestFixture]
    public class LoginViewModelTests
    {
        // Тестовый метод для проверки правильной реализации конструктора вью-модели
        [Test]
        public void TestViewModelConstructor()
        {
            var stub = new LastUsernameProviderStub();

            // "моделируем" внешнее окружение
            stub.UserName = "Jon Skeet"; // Ух-ты!!
            var vm = new LoginViewModel(stub);

            // Проверяем состояние тестируемого класса
            Assert.That(vm.UserName, Is.EqualTo(stub.UserName));
        }

        // Проверяем, что при вызове метода Login будет сохранено имя последнего пользователя
        [Test]
        public void TestLogin()
        {
            var mock = new LastUsernameProviderMock();
            var vm = new LoginViewModel(mock);

            // Изменяем состояние вью-модели путем изменения ее свойства
            vm.UserName = "Bob Martin";
            // А затем вызываем метод Login
            vm.Login();
            // Теперь мы проверяем, что был вызван метод SaveLastUserName
            Assert.That(mock.SavedUserName, Is.EqualTo(vm.UserName));
        }
    }


}
