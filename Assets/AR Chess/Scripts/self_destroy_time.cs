using UnityEngine;
using System.Collections;

public class self_destroy_time : MonoBehaviour {
	
		private GameObject Obj;
		private float elapsed;
		public float duration=1.0f;
		public int replacement=0;
		public bool fade=false;
		public float time_to_fade=0.5f;
		private float dincrem;
		
		private Color col_rend;
		private Renderer rend_for_alpha;
		
		public bool has_render=false;

		// Use this for initialization
		void Start () {
			Obj=gameObject;
			elapsed= 0;
			dincrem=duration-time_to_fade;
			
			/* INTENTO ALPHA
			if(Obj.renderer!=null)
			{
				has_render=true;
				col_rend=Obj.renderer.material.color;
			}
			else
			{
				has_render=false;
			}
			*/
			

			
		}
		
		// Update is called once per frame
		
		
		
		
		void Update () 
		{
			elapsed+=Time.deltaTime;
			
			if( elapsed > duration)
				{
				
				
    				//iTween.FadeTo( Obj , iTween.Hash( "alpha" , 0.0f , "time" , .3 , "easeType", "easeInSine") );
					//recolocar el texto si necesario
					if(replacement ==0)
					{
						Destroy(Obj);
					}
					else
					{
						//Obj.position=;
					}
					
				}	
				
			/*if(fade==true && elapsed>time_to_fade && has_render==true)
			{
				
				col_rend.a=(duration-elapsed) / (dincrem );				
			}*/
		
			
		}
		
		
		
	
	
		
		
		
}
	