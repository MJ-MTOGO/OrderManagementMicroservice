using OrderManagementService.Application.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementServiceTests.UnitTest
{
    public class CreateOrderValidatorTests
    {
        // Test Street BVA and EP ---------------------Street must not be empty and must be between 1 and 255 characters.--------------------------------------
        [Fact]
        public void ValidateStreet_ShouldNotThrow_WhenStreetIsAtMinimumLength() // BVA
        {
            // Arrange
            var street = "A"; // 1 character (minimum)

            // Act & Assert
            var exception = Record.Exception(() => CreateOrderValidator.ValidateStreet(street));
            Assert.Null(exception);
        }

        [Fact]
        public void ValidateStreet_ShouldNotThrow_WhenStreetIsAtMaximumLength() // BVA
        {
            // Arrange
            var street = new string('A', 255); // 255 characters (maximum)

            // Act & Assert
            var exception = Record.Exception(() => CreateOrderValidator.ValidateStreet(street));
            Assert.Null(exception);
        }

        [Fact]
        public void ValidateStreet_ShouldThrowException_WhenStreetExceedsMaximumLength() // BVA
        {
            // Arrange
            var street = new string('A', 256); // 256 characters (exceeds maximum)

            // Act & Assert
            Assert.Throws<ArgumentException>(() => CreateOrderValidator.ValidateStreet(street));
        }

        [Fact]
        public void ValidateStreet_ShouldThrowException_WhenStreetIsEmpty() // EP
        {
            // Arrange
            var street = "";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => CreateOrderValidator.ValidateStreet(street));
        }

        [Fact]
        public void ValidateStreet_ShouldNotThrow_WhenStreetIsValid() // EP
        {
            // Arrange
            var street = "123 Main St"; // Valid partition

            // Act & Assert
            var exception = Record.Exception(() => CreateOrderValidator.ValidateStreet(street));
            Assert.Null(exception);
        }
        // Test City BVA and EP ----------------------City must not be empty.-------------------------------------

        [Fact]
        public void ValidateCity_ShouldThrowException_WhenCityIsEmpty() // EP
        {
            // Arrange
            var city = "";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => CreateOrderValidator.ValidateCity(city));
        }

        [Fact]
        public void ValidateCity_ShouldNotThrow_WhenCityIsValid() // EP
        {
            // Arrange
            var city = "Copenhagen";

            // Act & Assert
            var exception = Record.Exception(() => CreateOrderValidator.ValidateCity(city));
            Assert.Null(exception);
        }

        // Test PostalCode BVA and EP ----------------------PostalCode must be exactly 4 numeric digits.-------------------------------------


        [Fact]
        public void ValidatePostalCode_ShouldNotThrow_WhenPostalCodeIsExactlyFourDigits() // BVA
        {
            // Arrange
            var postalCode = "1234"; // 4 digits (valid)

            // Act & Assert
            var exception = Record.Exception(() => CreateOrderValidator.ValidatePostalCode(postalCode));
            Assert.Null(exception);
        }

        [Fact]
        public void ValidatePostalCode_ShouldThrowException_WhenPostalCodeHasFewerThanFourDigits() // BVA
        {
            // Arrange
            var postalCode = "123"; // 3 digits (invalid)

            // Act & Assert
            Assert.Throws<ArgumentException>(() => CreateOrderValidator.ValidatePostalCode(postalCode));
        }

        [Fact]
        public void ValidatePostalCode_ShouldThrowException_WhenPostalCodeHasMoreThanFourDigits() // BVA
        {
            // Arrange
            var postalCode = "12345"; // 5 digits (invalid)

            // Act & Assert
            Assert.Throws<ArgumentException>(() => CreateOrderValidator.ValidatePostalCode(postalCode));
        }

        [Fact]
        public void ValidatePostalCode_ShouldThrowException_WhenPostalCodeIsNonNumeric() // EP
        {
            // Arrange
            var postalCode = "ABCD";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => CreateOrderValidator.ValidatePostalCode(postalCode));
        }

        [Fact]
        public void ValidatePostalCode_ShouldNotThrow_WhenPostalCodeIsValid() // EP
        {
            // Arrange
            var postalCode = "5678";

            // Act & Assert
            var exception = Record.Exception(() => CreateOrderValidator.ValidatePostalCode(postalCode));
            Assert.Null(exception);
        }

       


    }
}
