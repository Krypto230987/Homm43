using ChatClient_;
using static ChatClient_.Client;

Client c = new Client();

await c.RunAsync("localhost", 11000);
var client = new Client();

await client.RunAsync("localhost", 11000); 