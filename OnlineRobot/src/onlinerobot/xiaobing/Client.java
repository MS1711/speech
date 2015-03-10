package onlinerobot.xiaobing;

import java.io.*;
import java.net.*;

public class Client {
	
	public static String run(String prefix)
	{
		 Socket socket = null; 
		 String res="";
		try {  
			    socket = new Socket("127.0.0.1", 8000);  
			    
			    OutputStream netOut = socket.getOutputStream();  
			    DataOutputStream doc = new DataOutputStream(netOut);  
			    DataInputStream in = new DataInputStream(socket.getInputStream());  
			    
			    doc.writeUTF(prefix);  
			    res = in.readUTF();  
			  //  System.out.println(res);  
			    
			    doc.close();  
			    in.close();  
			    
			    
			} 
		catch (UnknownHostException e) 
		{  
			    e.printStackTrace();  
		} 
		catch (IOException e)
		{  
			    e.printStackTrace();  
		} 
		finally 
		{  
		    if (socket != null) 
		    {  
		        try
		        {  
		            socket.close();  
		        } 
		        catch (IOException e) 
		        {  
		        }  
		    }  
		}
		return res;
	}

  public static void main(String args[]) {
	 
	  String input = "你和小娜谁聪明";
      String output = "";
	  Client client = new Client();
      output = client.run(input);
      System.out.println("client:" + output); 
  }
}
