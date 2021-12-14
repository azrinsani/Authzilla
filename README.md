# Authzilla

Authzilla is a MicroService (Server) which wraps around [Identity Server 4](https://identityserver4.readthedocs.io/) 
transforming it into a much simpler out-of-the-box solution. 
Installation and setup is expected to be a breeze, like how one installs any Commercial Service App.

## Why Authzilla?
Identity Server is complicated. I have a theory that it was purposely made that way so support/consultancy work can be sold to corporate customers.
People need to make a living right?
But not everyone is corporate rich. Authzilla aims to solve the pain of setting up Identity Servers, by wrapping and packaging it as a 
simple installable service named Authzilla


## Do we still need Authzilla or Identity Server anyway?
Some people say Identity Server, Authzilla, or anything similar is no longer necessary since there are so many cloud solutions out there.
For example, [Firebase Authentication](https://firebase.google.com/docs/auth) is 100% Free, [AWS Cognito](https://aws.amazon.com/cognito/) 
is free for 50,000 Users and so is
[Microsoft Azure Active Directory External Identities](https://azure.microsoft.com/en-au/services/active-directory/external-identities), 
it is [free for 50,000 Users](https://azure.microsoft.com/en-us/pricing/details/active-directory/external-identities/) as well. 

The thing is, these cloud solutions only provide 1 out of the 5 [OAuth 2.0 Grant types](https://oauth.net/2/grant-types/), 
i.e. they only provide the [Authorization Code/PKCE Flow](https://auth0.com/docs/authorization/flows/authorization-code-flow).
If one requires server-to-server authentication, typical for Microservices, the [Client Credentials Flow](https://auth0.com/docs/authorization/flows/client-credentials-flow)
is necessary. Firebase Authentication does not provide this. 

## Project Status
This project is being abandoned due to other commitments. Feel free to take it up!


