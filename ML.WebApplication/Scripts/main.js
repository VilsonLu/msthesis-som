var app = angular.module("mlApp", []);

app.controller("configController",
    function ($scope, $http) {


        $scope.Configuration = {
            Epoch : 3,
            LearningRate: 0.5,
            Width: 0,
            Height: 0,
            FilePath: ""
        }

        $scope.TrainModel = function () {

            var data = $scope.Configuration;
            var requestParams = {
                Epoch: data.Epoch,
                LearningRate: data.LearningRate,
                Width: data.Width,
                Height: data.Height,
                FilePath: ""
            }

            var post = $http.post("http://localhost:49621/api/ML/GetTrainSOM", requestParams);

            post.then(function(response) {
                console.log("success call");
            });
        } 

    });