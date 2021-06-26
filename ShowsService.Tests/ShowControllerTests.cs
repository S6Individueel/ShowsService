using FakeItEasy;
using ShowsService.Controllers;
using ShowsService.DTOs;
using ShowsService.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ShowsService.Tests
{
    public class ShowControllerTests
    {
        int count;
        string key;
        IShowRepository showRepo; //Fakes a repository to initialize the controller with
        IList<ShowDTO> fakeShows;
        ShowController showController;
        public ShowControllerTests()
        {
            count = 10;
            showRepo = A.Fake<IShowRepository>(); //Fakes a repository to initialize the controller with
            fakeShows = (IList<ShowDTO>)A.CollectionOfDummy<ShowDTO>(10).AsEnumerable(); //Makes a dummy collection of the topmovietype
        }

        [Fact]
        public async Task Read_Top_Ten_Movies()
        {
            //Arange
            key = "Movie";
            A.CallTo(() => showRepo.GetTrendingShowsAsync(key)).Returns(await Task.FromResult(fakeShows));//Configures the call to return the faked data, making it independent from the API and testing pure code.
            showController = new ShowController(showRepo);
            
            //Act
            var result = await showController.GetTrendingMoviesAsync(); //Makes the call
            
            //Assert
            Assert.Equal(count, result.Count()); //Checks if the list is filled
        }

        [Fact]
        public async Task Read_Top_Ten_Anime()
        {
            //Arange
            key = "Anime";
            A.CallTo(() => showRepo.GetTrendingShowsAsync(key)).Returns(await Task.FromResult(fakeShows));//Configures the call to return the faked data, making it independent from the API and testing pure code.
            showController = new ShowController(showRepo);
            
            //Act
            var result = await showController.GetTrendingAnimesAsync(); //Makes the call
            
            //Assert
            Assert.Equal(count, result.Count()); //Checks if the list is filled
        }

        [Fact]
        public async Task Set_Show_Async()
        {
            //Arange
            key = "Cache";
            string cacheString = "{'name': 'cacheString'}";
            string nullResult = null;
            A.CallTo(() => showRepo.GetShowAsync(key)).Returns(await Task.FromResult(nullResult));//Configures the call to return the faked data, making it independent from the API and testing pure code.
            A.CallTo(() => showRepo.SetShowAsync(key, cacheString)).Returns(await Task.FromResult(cacheString));
            
            //Act
            var result = await showRepo.SetShowAsync(key, cacheString); //Makes the call

            //Assert
            Assert.NotNull(result); //Checks if the result is not null
        }

        [Fact]
        public async Task Get_Show_Async()
        {
            //Arange
            key = "Cache";
            string cacheString = "{'name': 'cacheString'}";
            A.CallTo(() => showRepo.GetShowAsync(key)).Returns(await Task.FromResult(cacheString));//Configures the call to return the faked data, making it independent from the API and testing pure code.

            //Act
            var result = await showRepo.GetShowAsync(key); //Makes the call

            //Assert
            Assert.NotNull(result); //Checks if the result is not null
        }
    }
}
