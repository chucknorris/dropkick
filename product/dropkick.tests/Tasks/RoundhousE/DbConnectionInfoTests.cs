using System;
using System.Collections.Generic;

using dropkick.Prompting;
using dropkick.Tasks.RoundhousE;

using NUnit.Framework;

namespace dropkick.tests.Tasks.RoundhousE
{
    [TestFixture]
    public class DbConnectionInfoTests
    {
        private DbConnectionInfo _info;
        private MockPromptService _prompt;

        [SetUp]
        public void Setup()
        {
            _prompt = new MockPromptService();
            _info = new DbConnectionInfo(_prompt) { Server = "foo", DatabaseName = "bar" };
        }

        [Test]
        public void use_integrated_security_if_no_username()
        {
            var cs = _info.BuildConnectionString();

            Assert.That(cs, Contains.Substring("Integrated Security=True"));
            Assert.That(_info.WillPromptForUserName() == false);
            Assert.That(_info.WillPromptForPassword() == false);
        }

        [Test]
        public void use_credentials_if_supplied()
        {
            _info.UserName = "bob";
            _info.Password = "pass";
            var cs = _info.BuildConnectionString();

            Assert.That(cs, Contains.Substring("User ID=bob"));
            Assert.That(cs, Contains.Substring("Password=pass"));
            Assert.That(_info.WillPromptForUserName() == false);
            Assert.That(_info.WillPromptForPassword() == false);
        }

        [Test]
        public void prompt_for_password_if_username_supplied_without_password()
        {
            _info.UserName = "bob";
            _info.BuildConnectionString();

            Assert.That(_prompt.PromptWasCalled);
            Assert.That(_info.WillPromptForUserName() == false);
            Assert.That(_info.WillPromptForPassword());
        }

        [Test]
        public void do_not_prompt_for_password_if_supplied()
        {
            _info.UserName = "bob";
            _info.Password = "pass";
            _info.BuildConnectionString();

            Assert.That(_prompt.PromptWasCalled == false);
            Assert.That(_info.WillPromptForUserName() == false);
            Assert.That(_info.WillPromptForPassword() == false);
        }
        
        [Test]
        public void prompt_if_username_with_question_mark_supplied()
        {
            _info.UserName = "?";
            _info.Password = "pass";
            _info.BuildConnectionString();

            Assert.That(_prompt.PromptWasCalled);
            Assert.That(_info.WillPromptForUserName());
            Assert.That(_info.WillPromptForUserName());
            Assert.That(_info.WillPromptForPassword() == false);
        }

        [Test]
        public void use_credentials_supplied_by_user_when_prompted()
        {
            _info.UserName = "?";
            _info.Password = "?";
            _prompt.SetupResponse("myuser");
            _prompt.SetupResponse("mypassword");

            var cs = _info.BuildConnectionString();

            Assert.That(cs, Contains.Substring("User ID=myuser"));
            Assert.That(cs, Contains.Substring("Password=mypassword"));
            Assert.That(_info.WillPromptForUserName());
            Assert.That(_info.WillPromptForPassword());
        }

        private class MockPromptService : PromptService
        {
            public bool PromptWasCalled = false;
            private Queue<string> _responses;

            public void SetupResponse(string response)
            {
                _responses = _responses ?? new Queue<string>();
                _responses.Enqueue(response);
            }
            
            public string Prompt(string nameToDisplay)
            {
                PromptWasCalled = true;
                return _responses == null ? "" : _responses.Dequeue();
            }

            public T Prompt<T>() where T : new()
            {
                throw new NotImplementedException();
            }

            public void Tick()
            {
                throw new NotImplementedException();
            }
        }
    }
}