# RSS-client
RSS client (supports Atom 1.0 and RSS 2.0)

This app is created for reading news from your favourite feeds. 
It generates a single feed (shown as HTML page) based on updates from subscribtions (some of them are stored as samples)
and gives an opportunity to read articles either from source or from HTML-pages, generated by client.
The client also supports the following functions: 
sorting feeds by their categories, storing articles as 'Favourites', sharing news via social media. 
The algorithm for feed generation is pretty simple: 
-> Get XML document via HTTP 
-> Parse it into one of the supported versions 
-> Sort it into one of the categories along the way 
-> Repeat for every website 
-> Generate HTML representation of the whole feed 
-> Voila 
