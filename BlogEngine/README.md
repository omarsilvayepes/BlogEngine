# Project Title

Blog Engine .NET APP.

## Description

Blog Engine / CMS backend API, that allows to create, edit and publish text based posts, with an approval flow where two different user types may interact.

## Getting Started

### Dependencies

* Microservice developed in .Net core framework 6
* Database MongoDB Cloud:MongoDB Atlas
* Mongo Driver
* Microsoft AspNetCore Authentication JwtBearer


### Executing program

* Heroku /localhost /docker / iis /console

```
code blocks for commands
```

## Help

* The solution it was deploy in Heroku (https://app-blogengine.herokuapp.com/) so it can be tested without local install.

* The application as well runs on localhost  (https://localhost:7168/) and is   tested with Postman , Swagger or any API testing platform.

* Application EndPoints for test it:

POST:

* https://app-blogengine.herokuapp.com/api/User/Login
* https://app-blogengine.herokuapp.com/api/User/Register
* https://app-blogengine.herokuapp.com/api/Post/CreatePost
* https://app-blogengine.herokuapp.com/api/Post/UpdatePost
* https://app-blogengine.herokuapp.com/api/Post/AddCommentById
* https://app-blogengine.herokuapp.com/api/Post/AddCommentRejectPost
* https://app-blogengine.herokuapp.com/api/Post/SubmitPost
* https://app-blogengine.herokuapp.com/api/Post/ApprovePendingPost
* https://app-blogengine.herokuapp.com/api/Post/RejectPendingPost

GET:

* https://app-blogengine.herokuapp.com/api/Post/getPublishedPosts
* https://app-blogengine.herokuapp.com/api/Post/getCreateAndPendingPosts
* https://app-blogengine.herokuapp.com/api/Post/getPendingPosts
* https://app-blogengine.herokuapp.com/api/Post/getCommentsRejectPost

```
command to run if program contains helper info
```

## Authors

Omar Silva Yepes 
[@Omar](omar.yepes@hotmail.com)

## Version History

* 0.1
    * Initial Release

## Additional Information

* Estimated construction time was approximately 25 hours.
* Sample credentials for different types of user:

| *User/Role*     | Pass                                             |
| ------------ | ---------------------------------------------------- |
| omar45/Writer | 123456        |
| omar46/Public           | 123456      |
| omar47/Editor             | 123456|

* Postman collections are attached to test the services.