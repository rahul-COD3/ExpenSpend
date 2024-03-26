using ExpenSpend.Domain.DTOs.Friends;
using ExpenSpend.Domain.DTOs.Friends.Enums;
using ExpenSpend.Domain.Models.Friends;
using ExpenSpend.Service.Contracts;
using ExpenSpend.Service.Models;
using ExpenSpend.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ExpenSpend.Test.Friends
{
    public class FriendshipController_Test
    {
        private readonly Mock<IFriendAppService> friendAppServiceMock;
        private readonly FriendshipController controller;

        public FriendshipController_Test()
        {
            // use get required service method to get the service friendAppService

            friendAppServiceMock = new Mock<IFriendAppService>();
            controller = new FriendshipController(friendAppServiceMock.Object);
        }

        [Fact]
        public async void GetFriendshipsAsync_ReturnsOk()
        { 
            // Arrange
            var expectedSource = new Response(new List<GetFriendshipDto>());
            friendAppServiceMock.Setup(x => x.GetFriendshipsAsync()).ReturnsAsync(expectedSource);
            // Act
            var result = await controller.GetAllFriendships();
            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public async void AcceptFriendship_FriendshipSatatusShouldBeBlocked()
        {
            var friendship = new Friendship(Guid.NewGuid(), Guid.NewGuid(), FriendshipStatus.Accepted, DateTime.Now);
            var expectedSource = new Response(friendship);
            friendAppServiceMock.Setup(x => x.BlockAsync(friendship.Id)).ReturnsAsync(expectedSource);
            // Act
            var result = await controller.BlockFriendship(friendship.Id);
            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public async void CreateFriendship_FriendshipShouldBeCreated()
        {
            var friendship = new CreateFriendshipDto { RecipientId = Guid.NewGuid() };
            var expectedSource = new Response(friendship);
            friendAppServiceMock.Setup(x => x.CreateFriendAsync(friendship.RecipientId)).ReturnsAsync(expectedSource);
            // Act
            var result = await controller.CreateFriendship(friendship);
            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
        // bad request when friendship is not created
        [Fact]
        public async void CreateFriendship_FriendshipShouldNotBeCreated()
        {
            var friendship = new CreateFriendshipDto { RecipientId = Guid.NewGuid() };
            var expectedSource = new Response("Friendship not created");
            friendAppServiceMock.Setup(x => x.CreateFriendAsync(friendship.RecipientId)).ReturnsAsync(expectedSource);
            // Act
            var result = await controller.CreateFriendship(friendship);
            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }


    }
}
