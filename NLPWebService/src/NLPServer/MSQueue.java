/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package NLPServer;
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
public class MSQueue {
    Queue queue = null;
    public void InitMSQueue() {
        try {
            String fullname= "MININT-CPNFUTJ\\HouseCommandQueue";
            //ionic.Msmq.Queue.delete(fullname);
            String qLabel="Created by WebService from v-feiche";
            boolean transactional= false;
            queue= Queue.create(fullname, qLabel, transactional);
        }catch (Exception ex1) {
            System.out.println("Failure in creating queue");
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
            String mLabel="Created by WebService from v-feiche";
            String correlationID= "L:none";
            //java.util.Calendar cal= java.util.Calendar.getInstance(); // current time & date
            //java.text.SimpleDateFormat df = new java.text.SimpleDateFormat("yyyy-MM-dd HH:mm:sszzz");
            String body= msgText;
            Message msg= new Message(body, mLabel, correlationID);
            queue.send(msg);
        }
        catch (MessageQueueException ex1) {
            System.out.println("Send failure: " + ex1);
        } catch (UnsupportedEncodingException ex) {
           System.out.println("Send failure: " + ex);
        }
    }
    
}
