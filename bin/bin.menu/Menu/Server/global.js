function doc_init()
{
	c = document.body;
	c.oncontextmenu = g_return_false;
	c.onkeydown 	= g_onkeydown;
	c.onhelp 		= g_disable_help;
}

function g_return_false()
{
	return false;
}

function g_disable_help()
{
	window.event.returnValue = false;
}

function show(col)
{
	var str = "";
	
	if(col == null)
		return "null";
	
	for(i = 0; i < col.length; i++)
	{
		str += col.item(i).tagName + "<br>";
	}
	
	return str;
}


function g_onkeydown()
{
	var name;

//	alert(window.event.keyCode);
	if(window.event.keyCode >= 112 && window.event.keyCode <= (112 + 12))
	{
		name = "F" + (window.event.keyCode - 111);
	}

	if(window.event.keyCode >= 49 && window.event.keyCode <= 59)
	{
		name = "NUM" + (window.event.keyCode - 48);
	}

	if(window.event.keyCode >= 96 && window.event.keyCode <= 105)
	{
		name = "NUM" + (window.event.keyCode - 96);
	}
	
//	alert(name);
	window.event.returnValue = false;
//	window.event.keyCode = 0;
	// F5の更新を効かないようにする
	if(name == "F5")
	{
		window.event.keyCode = 0;
	}
	
	if(name == null)
	{
//		alert("not found");
		return;
	}

	var c;
	if(window.top.frames.length > 0)
	{
		//	フレームを全て探す
		for(i = 0; i < window.top.frames.length; i++)
		{
			c = window.top.frames(i).document.all.item(name);
			if(c != null)
			{
				break;
			}
		}
	}
	else
	{
		//	フレームが無い場合ドキュメントを探す
		c = window.top.document.all.item(name);
	}
	
	if(c != null)
	{
		c.focus();
		c.click();
	}
}

function run_syain()
{
	location="exec://SetHenkosha.exe?wait=1&child=0";
	
	location.href = "menu_mnt.html";
}
