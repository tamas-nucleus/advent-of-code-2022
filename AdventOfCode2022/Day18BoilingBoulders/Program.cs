using Day18BoilingBoulders;

var droplet = new Droplet();
droplet.Load("input.txt");
droplet.MeasureSurface();
Console.WriteLine($"The surface of the droplet is about {droplet.Surface} units.");
Console.WriteLine($"The external surface of the droplet is about {droplet.ExternalSurface} units.");
