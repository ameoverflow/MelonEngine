tiled.assetAboutToBeSaved.connect(function(asset) {
    if (tiled.versionLessThan("1.10.2"))
    {
        tiled.error("Script was written for Tiled 1.10.2. You're running Tiled " + tiled.version);
    }

    if (!asset.isTileMap) return;

    if (asset.tileWidth != 10 || asset.tileHeight != 10)
    {
        tiled.error("Invalid tile size");
    }

    if (asset.orientation != 1)
    {
        tiled.error("Invalid map orientation. It needs to be set to Orthogonal");
    }

    asset.layers.forEach(element => {
        if (element.isObjectLayer)
        {
            //Check if object types are valid
            element.objects.forEach(obj => {
                if (obj.shape != MapObject.Rectangle)
                {
                    tiled.warn(`Object '${obj.name}' with ID ${obj.id} is an unsupported object shape. Object won't be parsed by engine.`);
                }
                if (obj.className == undefined) //generic object
                {
                    if (obj.properties()["Collide"] == undefined || typeof(obj.properties()["Collide"]) != Boolean)
                    {
                        tiled.error(`Object '${obj.name}' with ID ${obj.id} doesn't have required property: Collide (bool)`);
                    }
                }
            });
        }
    });

    if (asset.properties()["Title"] == undefined)
    {
        tiled.error("Map title is not defined");
    }

    if (asset.properties()["Music"] == undefined)
    {
        tiled.warn("Map doesn't have music");
    }
});