const sql = require('mssql');

module.exports = function (context, req) {
    context.log('JavaScript HTTP trigger function processed a request.');

    var id = (req.query.ID || req.body.ID);
    var name = (req.query.Name || req.body.Name);
    var botId = (req.query.BotId || req.body.BotId);
    var botName = (req.query.BotName || req.body.BotName);
    var serviceUrl = (req.query.ServiceUrl || req.body.ServiceUrl);
    var token = (req.query.token || req.body.token);
    var conversationId = (req.query.ConversationId || req.body.ConversationId);
    var channelId = (req.query.ChannelId || req.body.ChannelId);

    if (id && name && botId && botName && serviceUrl) {

        try {
            const pool = new sql.ConnectionPool({
                user: 'sqlroot',
                password: 'Pass@word!',
                server: 'ndbsql.database.windows.net',
                database: 'ndbsql'
            })

            const result = await sql.query`INSERT INTO dbo.users_v2 VALUES ${id} ${name} ${botId} ${botName} ${serviceUrl} ${token} ${conversationId} ${channelId};`
            console.dir(result)
        } catch (err) {
            console.log(err)
        }

    } else {
        context.res = {
            status: 400,
            body: "ID, Name, BotId, BotName, ServiceUrl, Token are requird on the query string or in the request body"
        };
    }
    context.done();
};