using NUnit.Framework;
using LibraryApp.Models;
using LibraryApp.Services;
using LibraryApp.Exceptions;
using System;
using System.Reflection;

namespace LibraryApp.Test
{
    public class LibraryServiceTest
    {
        private LibraryService _service;


        private FakeBookService _fakeBookService;
        private FakeDeliveryService _fakeDeliveryService;
        private FakePurchaseService _fakePurchaseService;

        [SetUp]
        public void SetUp()
        {
            _fakeBookService = new FakeBookService();
            _fakeDeliveryService = new FakeDeliveryService();
            _fakePurchaseService = new FakePurchaseService();

            _service = new LibraryService();

            SetPrivateField("_bookService", _fakeBookService);
            SetPrivateField("_deliveryService", _fakeDeliveryService);
            SetPrivateField("_purchaseService", _fakePurchaseService);
        }

        private void SetPrivateField(string fieldName, object value)
        {
            FieldInfo field = typeof(LibraryService).GetField(
                fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance
            );

            field.SetValue(_service, value);
        }

        private Book CreateBook(int numberOfCopies)
        {
            return new Book
            {
                Id = Guid.NewGuid(),
                Title = "Test title",
                Author = "Test author",
                Price = 1000,
                NumberOfCopies = numberOfCopies
            };
        }

       
        [Test]
        public void GetMemberDiscount_HighActivity_WithoutPenalty_Returns25()
        {
            int result = _service.GetMemberDiscount(
                20,
                5,
                false,
                ActivityFrequency.High
            );

            Assert.That(result, Is.EqualTo(25));
        }

        [Test]
        public void GetMemberDiscount_HighActivity_WithPenalty_Returns20()
        {
            int result = _service.GetMemberDiscount(
                20,
                5,
                true,
                ActivityFrequency.High
            );

            Assert.That(result, Is.EqualTo(20));
        }

        [Test]
        public void GetMemberDiscount_HighActivity_NotEnoughPurchases_Returns10()
        {
            int result = _service.GetMemberDiscount(
                20,
                4,
                false,
                ActivityFrequency.High
            );

            Assert.That(result, Is.EqualTo(10));
        }

        [Test]
        public void GetMemberDiscount_HighActivity_BookPriceNotGreaterThan10_Returns10()
        {
            int result = _service.GetMemberDiscount(
                10,
                5,
                false,
                ActivityFrequency.High
            );

            Assert.That(result, Is.EqualTo(10));
        }

        [Test]
        public void GetMemberDiscount_RegularActivity_BookPriceGreaterThan25_Returns15()
        {
            int result = _service.GetMemberDiscount(
                30,
                5,
                false,
                ActivityFrequency.Regular
            );

            Assert.That(result, Is.EqualTo(15));
        }

        [Test]
        public void GetMemberDiscount_RegularActivity_MoreThan12Purchases_Returns15()
        {
            int result = _service.GetMemberDiscount(
                20,
                13,
                false,
                ActivityFrequency.Regular
            );

            Assert.That(result, Is.EqualTo(15));
        }

        [Test]
        public void GetMemberDiscount_RegularActivity_NoConditionSatisfied_Returns10()
        {
            int result = _service.GetMemberDiscount(
                20,
                5,
                false,
                ActivityFrequency.Regular
            );

            Assert.That(result, Is.EqualTo(10));
        }

        [Test]
        public void GetMemberDiscount_LowActivity_Returns0()
        {
            int result = _service.GetMemberDiscount(
                50,
                20,
                false,
                ActivityFrequency.Low
            );

            Assert.That(result, Is.EqualTo(0));
        }

        
        [Test]
        public void DoPurchaseCalculation_NoRequests_ThrowsNoRequestsForCalculationException()
        {
            Book book = CreateBook(5);

            _fakeBookService.BookRequestInfo = new BookRequestInfo
            {
                NumberOfTotalRequests = 0,
                PercentOfUnprocessedRequests = 0
            };

            _fakeDeliveryService.DeliveryType = DeliveryType.Local;

            Action action = () => _service.DoPurchaseCalculation(book);

            Assert.That(
                action,
                Throws.TypeOf<NoRequestsForCalculationException>()
            );
        }

        [Test]
        public void DoPurchaseCalculation_Oversea_UnprocessedOver80_CopiesGreaterOrEqual10_Purchases20()
        {
            Book book = CreateBook(10);

            _fakeBookService.BookRequestInfo = new BookRequestInfo
            {
                NumberOfTotalRequests = 5,
                PercentOfUnprocessedRequests = 90
            };

            _fakeDeliveryService.DeliveryType = DeliveryType.Oversea;

            _service.DoPurchaseCalculation(book);

            Assert.That(_fakePurchaseService.CreatedPurchase, Is.Not.Null);
            Assert.That(
                _fakePurchaseService.CreatedPurchase.NumberOfCopiesToBePurchased,
                Is.EqualTo(20)
            );
        }

        [Test]
        public void DoPurchaseCalculation_Oversea_UnprocessedOver80_CopiesLessThan10_Purchases15()
        {
            Book book = CreateBook(9);

            _fakeBookService.BookRequestInfo = new BookRequestInfo
            {
                NumberOfTotalRequests = 5,
                PercentOfUnprocessedRequests = 90
            };

            _fakeDeliveryService.DeliveryType = DeliveryType.Oversea;

            _service.DoPurchaseCalculation(book);

            Assert.That(_fakePurchaseService.CreatedPurchase, Is.Not.Null);
            Assert.That(
                _fakePurchaseService.CreatedPurchase.NumberOfCopiesToBePurchased,
                Is.EqualTo(15)
            );
        }

        [Test]
        public void DoPurchaseCalculation_Oversea_UnprocessedEqual80_Purchases10()
        {
            Book book = CreateBook(10);

            _fakeBookService.BookRequestInfo = new BookRequestInfo
            {
                NumberOfTotalRequests = 5,
                PercentOfUnprocessedRequests = 80
            };

            _fakeDeliveryService.DeliveryType = DeliveryType.Oversea;

            _service.DoPurchaseCalculation(book);

            Assert.That(_fakePurchaseService.CreatedPurchase, Is.Not.Null);
            Assert.That(
                _fakePurchaseService.CreatedPurchase.NumberOfCopiesToBePurchased,
                Is.EqualTo(10)
            );
        }

        [Test]
        public void DoPurchaseCalculation_International_UnprocessedOver50_Purchases15()
        {
            Book book = CreateBook(5);

            _fakeBookService.BookRequestInfo = new BookRequestInfo
            {
                NumberOfTotalRequests = 5,
                PercentOfUnprocessedRequests = 60
            };

            _fakeDeliveryService.DeliveryType = DeliveryType.International;

            _service.DoPurchaseCalculation(book);

            Assert.That(_fakePurchaseService.CreatedPurchase, Is.Not.Null);
            Assert.That(
                _fakePurchaseService.CreatedPurchase.NumberOfCopiesToBePurchased,
                Is.EqualTo(15)
            );
        }

        [Test]
        public void DoPurchaseCalculation_International_TotalRequestsGreaterThan10_Purchases15()
        {
            Book book = CreateBook(5);

            _fakeBookService.BookRequestInfo = new BookRequestInfo
            {
                NumberOfTotalRequests = 11,
                PercentOfUnprocessedRequests = 40
            };

            _fakeDeliveryService.DeliveryType = DeliveryType.International;

            _service.DoPurchaseCalculation(book);

            Assert.That(_fakePurchaseService.CreatedPurchase, Is.Not.Null);
            Assert.That(
                _fakePurchaseService.CreatedPurchase.NumberOfCopiesToBePurchased,
                Is.EqualTo(15)
            );
        }

        [Test]
        public void DoPurchaseCalculation_International_NoConditionSatisfied_Purchases12()
        {
            Book book = CreateBook(5);

            _fakeBookService.BookRequestInfo = new BookRequestInfo
            {
                NumberOfTotalRequests = 10,
                PercentOfUnprocessedRequests = 50
            };

            _fakeDeliveryService.DeliveryType = DeliveryType.International;

            _service.DoPurchaseCalculation(book);

            Assert.That(_fakePurchaseService.CreatedPurchase, Is.Not.Null);
            Assert.That(
                _fakePurchaseService.CreatedPurchase.NumberOfCopiesToBePurchased,
                Is.EqualTo(12)
            );
        }

        [Test]
        public void DoPurchaseCalculation_LocalDelivery_Purchases10()
        {
            Book book = CreateBook(5);

            _fakeBookService.BookRequestInfo = new BookRequestInfo
            {
                NumberOfTotalRequests = 5,
                PercentOfUnprocessedRequests = 90
            };

            _fakeDeliveryService.DeliveryType = DeliveryType.Local;

            _service.DoPurchaseCalculation(book);

            Assert.That(_fakePurchaseService.CreatedPurchase, Is.Not.Null);
            Assert.That(
                _fakePurchaseService.CreatedPurchase.NumberOfCopiesToBePurchased,
                Is.EqualTo(10)
            );
        }
    }
}