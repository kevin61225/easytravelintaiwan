function FacebookInit() {
    FB.init({
        appId: '544818928892863', // App ID
        channelUrl: 'http://localhost:51145/', // Channel File
        status: true, // check login status
        cookie: true, // enable cookies to allow the server to access the session
        xfbml: true  // parse XFBML
    });
    console.log("init");
}

//function FacebookLogin() {
//    //FacebookInit();
//    FB.Event.subscribe('auth.authResponseChange', function (response) {
//        // Here we specify what we do with the response anytime this event occurs. 
//        if (response.status === 'connected') {
//            // The response object is returned with a status field that lets the app know the current
//            // login status of the person. In this case, we're handling the situation where they 
//            // have logged in to the app.
//            FacebookLogin();
//        }
//    });
//    FB.login(function (response) {
//        console.log(response);
//        if (response.authResponse) {
//            FB.api('/me', function (response) {
//                var fbID = response.id;
//                var fbName = response.name;
//                var fbEmail = response.email;          
//                console.log('login ! Good to see you, ' + fbName + ', ' + fbID + ', ' + 'Email = ' + fbEmail);
//                console.log(response);
//                SetProfilePicture(fbID);
//                location.href = '/Member/FacebookLogin?fbID=' + fbID + '&fbName=' + fbName + '&fbEmail=' + fbEmail + '&sex=' + response.gender;
//            });       
//        }
//    }, { scope: 'email' });
//}

//// Load the SDK asynchronously
//(function (d) {
//    var js, id = 'facebook-jssdk', ref = d.getElementsByTagName('script')[0];
//    if (d.getElementById(id)) { return; }
//    js = d.createElement('script'); js.id = id; js.async = true;
//    js.src = "//connect.facebook.net/en_US/all.js";
//    ref.parentNode.insertBefore(js, ref);
//}(document));


function SetProfilePicture(profileId) {
    if (profileId != "") {
        $('#ProfilePicture').html('<img src="' + 'https://graph.facebook.com/' + profileId + '/picture?width=40&height=60"></img>');
    }
    console.log("set!!");
}

function Logout() {
    FacebookLogout();
    location.href = '/Home/LogOut';
}

function FacebookLogout() {
    //FacebookInit();
    FB.getLoginStatus(statusResponse);
    console.log("Logged out !");
}

function statusResponse(response) {
    //FacebookInit();
    FB.logout(response.authResponse.accessToken);
}




