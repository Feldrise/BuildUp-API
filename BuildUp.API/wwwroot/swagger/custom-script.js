(function () {
    window.addEventListener("load", function () {
        setTimeout(function () {
            var logo = document.getElementsByClassName('link'); //For Changing The Link On The Logo Image
            logo[0].href = "https://new-talents.fr/";
            logo[0].target = "_blank";
            logo[0].children[0].alt = "Feldrise profile picture";
            logo[0].children[0].src = "https://avatars1.githubusercontent.com/u/15325126?s=460&u=c962f108a5443ae52f1d50b41dc399c0a34bfbe5&v=4"; //For Changing The Logo Image
        });
    });
})();