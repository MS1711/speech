/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package msmq;
import ionic.Msmq.Message;
import ionic.Msmq.Queue;
import ionic.Msmq.MessageQueueException;
import ionic.Msmq.TransactionType;
import java.io.UnsupportedEncodingException;
import java.util.logging.Level;
import java.util.logging.Logger;
/**
 *
 * @author v-feiche
 */
public class MSMQ {

    Queue queue = null;
    
    public void Create()
    {
        //System.load("F:\\NLPWebService2\\MsmqJava.dll");
        String qname= "jkjklohuddadfdddjjffdddgdddfg";
        String fullname= "MININT-CPNFUTJ\\" + qname;
        String qLabel="Created by " + ".java";
        boolean transactional= false;  // should the queue be transactional
        //return fullname;  
        try {
            queue= Queue.create(fullname, qLabel, transactional);
            //return "mymymy";
        } catch (MessageQueueException ex) {
            System.out.println("Fail");
        }    
    }

     private void checkQueue()
        throws MessageQueueException {
        if (queue==null)
            throw new MessageQueueException("create a queue first!\n",-1);
    }

    public void Send(String msgText)
    {
        try {
            checkQueue();
            // the transaction flag must agree with the transactional flavor of the queue.
            String mLabel="inserted by " + this.getClass().getName() + ".java" ;

            String correlationID= "L:none";
            //java.util.Calendar cal= java.util.Calendar.getInstance(); // current time & date
            //java.text.SimpleDateFormat df = new java.text.SimpleDateFormat("yyyy-MM-dd HH:mm:sszzz");
            String body= msgText;
            Message msg= new Message(body, mLabel, correlationID);
            queue.send(msg);
            System.out.println("dd");
        }
        catch (MessageQueueException ex1) {
            System.out.println("Put failure: " + ex1);
        } catch (UnsupportedEncodingException ex) {
            System.out.println("Fail");
        }
    }
}
