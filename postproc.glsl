uniform fragmentProcessor color_map;

uniform vec3 ChannelMixer = vec3(1, 1, 1);
uniform float Vignette = 0;
uniform float ColorDepth = 100;
uniform vec2 Parallax = vec2(0, 0);
uniform vec2 Resolution;
uniform float Strength = 0.01;
uniform float Separation  = 3;
uniform float Bloom = 0.35;
uniform vec3 BloomColor = vec3(1,1,1);
uniform float BloomSize = 10000.0;

vec4 samp(vec2 frag)
{
    vec4 c = sample(color_map, frag);
    float t = 0.75;
    if(c.r < t && c.g < t && c.b < t)
        c = vec4(0,0,0,0);
    return c;
}

vec4 blm(vec2 fragCoord)
{
    const float Pi = 6.28318530718; // Pi*2
    
    // GAUSSIAN BLUR SETTINGS {{{
    const float Directions = 16.0; // BLUR DIRECTIONS (Default 16.0 - More is better but slower)
    const float Quality = 6.0; // BLUR QUALITY (Default 4.0 - More is better but slower)
   
    vec2 Radius = BloomSize/Resolution.xy;
    
    // Normalized pixel coordinates (from 0 to 1)
    vec2 uv = fragCoord;
    // Pixel colour
    vec4 Color = samp(uv);
    
    // Blur calculations
    for( float d=0.0; d<Pi; d+=Pi/Directions)
    {
		for(float i=1.0/Quality; i<=1.0; i+=1.0/Quality)
        {
			Color += samp(uv+vec2(cos(d)*2,sin(d))*Radius*i);		
        }
    }
        // Output to screen
    Color /= Quality * Directions - 15.0;
  	return Color*vec4(BloomColor,1)*Bloom+sample(color_map, fragCoord);
}


vec4 main(vec2 frag) { 
  	
  	vec2 uv = frag;
  	//Bloom
  	vec3 col = blm(frag).rgb;
  
  	vec2 dist = (Resolution.xy/2-uv)/(Resolution.xy/2);


    

	//Chromatic Aberration
    //col.r = sample(color_map, vec2(uv.x+Strength,uv.y+Strength)).r;
    //col.g = sample(color_map, vec2(uv.x+Strength+Separation ,uv.y+Strength+(Separation) )).g;
    //col.b = sample(color_map, vec2(uv.x+Strength+(Separation *2.),uv.y+Strength+(Separation *2.))).b;
  
  	//Calculate Vignette
  	float v = 1-sqrt(pow(dist.x,2)+pow(dist.y,2))*Vignette;
  


  	//Apply Color Depth	
  	col = floor(col*ColorDepth)/ColorDepth;
  
  	//Apply Vignette to Color
   	col *= v;
  
  	//Apply Channel Mixer
  	col *= ChannelMixer; 
    


  
  	return vec4(col, 1);
}

