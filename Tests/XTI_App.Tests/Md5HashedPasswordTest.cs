using NUnit.Framework;

namespace XTI_App.Tests
{
    public sealed class Md5HashedPasswordTest
    {
        [Test]
        public void ShouldHashPassword()
        {
            var originalPassword = "Password1234";
            var factory = new Md5HashedPasswordFactory();
            var hashedPassword1 = factory.Create(originalPassword);
            Assert.That(hashedPassword1.Value(), Is.Not.EqualTo(originalPassword), "Should hash the password");
            var hashedPassword2 = factory.Create(originalPassword);
            Assert.That(hashedPassword2.Equals(hashedPassword1.Value()), Is.True, "Hashes of the same password should be equal");
        }
    }
}