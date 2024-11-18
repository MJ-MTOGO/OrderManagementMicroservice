using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementServiceTests.UnitTest
{
    public class OrderUnitTest
    {
        [Fact]
        public async Task Test123()
        {
            int number1 = 5;
            int number2 = 10;

            int result = (number1 + number2);

            Assert.Equal(15, result);
        }
    }
}
