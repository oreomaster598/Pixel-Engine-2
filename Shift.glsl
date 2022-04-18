uniform fragmentProcessor color_map;
uniform vec2 Shift;

vec2 shift(vec2 f, vec2 p)
{
  vec2 s = f+p;
  if(s.x > iResolution.x)
  	s.x=s.x - iResolution.x*floor(s.x/iResolution.x);
   if(s.x > iResolution.y)
  	s.y=s.y - iResolution.y*floor(s.y/iResolution.y);
  return s;
}

vec4 main(vec2 frag)
{
    return sample(color_map, shift(frag, Shift));
}