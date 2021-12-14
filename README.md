# Authzilla

Authzilla is a MicroService (Server) which wraps around [Identity Server 4](https://identityserver4.readthedocs.io/) 
transforming it into a much simpler out-of-the-box solution. 
Installation is expected to be a breeze, like how one installs any Commercial Service App.

## Why Authzilla?
Identity Server is complicated. I have a theory that it was purposely made that way so support/consultancy work can be sold to corporate customers.
People need to make a living right?
But not everyone is corporate rich. Authzilla aims to solve the pain of setting up Identity Server from cheapskates like myself, by wrapping it around an 
simple installable service.


## Do we still need Authzilla or Identity Server anyway?
Some people say Identity Server, Authzilla, or anything similar is no longer necessary since there are so many cloud solutions out there.
For example, [Firebase Authentication](https://firebase.google.com/docs/auth) is 100% Free,
and AWS Cognito is free for 50,000 Monthly Active Users. Those thay claim so have no idea what they are talking about.

The thing is, these cloud solutions only provide user 1 out of the 5 [OAuth 2.0 Grant types](https://oauth.net/2/grant-types/), 
i.e. they only provide the [Authorization Code/PKCE Flow](https://auth0.com/docs/authorization/flows/authorization-code-flow).
If one requires server-to-server authentication, typical for Microservices, the [Client Credentials Flow](https://auth0.com/docs/authorization/flows/client-credentials-flow)
is necessary. Firebase Authentication does not provide this. 

## Project Status
This project is being abandoned due to other commitments. Feel free to take it up!


