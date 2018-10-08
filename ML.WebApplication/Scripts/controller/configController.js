app.controller("configController",
    function ($scope, $http) {

        // Properties
        $scope.Configuration = {
            Epoch: 3,
            LearningRate: 0.5,
            Width: 0,
            Height: 0,
            FilePath: "",
            Labels: "",
            FeatureLabel: "",
            KMeans: 2
        };

        $scope.Files = [];

        $scope.Map = {};

        $scope.ShowLoader = true;


        // Methods
        $scope.TrainModel = function () {

            var data = $scope.Configuration;

            $scope.$broadcast("onShowLoader", { message: true });
       

            $http({
                method: "POST",
                url: "http://localhost:49621/api/ML/GetTrainSOM",
                headers: { "Content-Type": undefined },

                transformRequest: function(data) {
                    var formData = new FormData();
                    formData.append("model", angular.toJson(data.model));
                    for (var i = 0; i < data.files.length; i++) {
                        formData.append("file" + i, data.files[i]);
                    }
                    return formData;
                },
                data: { model: $scope.Configuration, files: $scope.Files }

            }).then(function(response) {
                $scope.Map = response.data.Model;
                $scope.$broadcast("onTrainedModel", { message: $scope.Map });
            });

            $scope.showModal = function () {
    
            }

            // Events
            $scope.$on("selectedFile", function (event, args) {
                $scope.$apply(function () {
                    //add the file object to the scope's files collection  
                    $scope.Files.push(args.file);
                });
            });  

            //var post = $http.post("http://localhost:49621/api/ML/GetTrainSOM", requestParams);

            //post.then(function (response) {
            //    
            //    console.log("success call");
            //});
        };

    });