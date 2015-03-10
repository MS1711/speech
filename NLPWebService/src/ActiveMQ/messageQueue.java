package ActiveMQ;

import java.util.logging.Level;
import java.util.logging.Logger;

import javax.jms.Connection;
import javax.jms.ConnectionFactory;
import javax.jms.DeliveryMode;
import javax.jms.Destination;
import javax.jms.JMSException;
import javax.jms.MessageProducer;
import javax.jms.Session;
import javax.jms.TextMessage;

import org.apache.activemq.ActiveMQConnection;
import org.apache.activemq.ActiveMQConnectionFactory;

import properties.Config;

public class messageQueue {
     ConnectionFactory connectionFactory;
     // Connection ：JMS 客户端到JMS Provider 的连接
     Connection connection = null;
     // Session： 一个发送或接收消息的线程
     Session session;
     // Destination ：消息的目的地;消息发送给谁.
     Destination destination;
     // MessageProducer：消息发送者
     MessageProducer producer;
    public messageQueue()
    {
        // ConnectionFactory ：连接工厂，JMS 用它创建连接
       
        // TextMessage message;
        // 构造ConnectionFactory实例对象，此处采用ActiveMq的实现jar
        connectionFactory = new ActiveMQConnectionFactory(
                ActiveMQConnection.DEFAULT_USER,
                ActiveMQConnection.DEFAULT_PASSWORD,
                Config.getInstance().get("mq_url", "tcp://localhost:61616"));
        try {
            connection = connectionFactory.createConnection();
            connection.start();
            // 获取操作连接
            session = connection.createSession(Boolean.TRUE,
                    Session.AUTO_ACKNOWLEDGE);
            // 获取session注意参数值xingbo.xu-queue是一个服务器的queue，须在在ActiveMq的console配置
            destination = session.createQueue("HouseCommand");
            producer = session.createProducer(destination);
            // 设置不持久化，此处学习，实际根据项目决定
            producer.setDeliveryMode(DeliveryMode.NON_PERSISTENT);
        } catch (JMSException ex) {
            Logger.getLogger(messageQueue.class.getName()).log(Level.SEVERE, null, ex);
        }
    }
    
    public void Sender(String msg)
    {
        TextMessage message;
         try {
              message = session.createTextMessage(msg);
              producer.send(message);
              session.commit();
         } catch (JMSException ex) {
             Logger.getLogger(messageQueue.class.getName()).log(Level.SEVERE, null, ex);
         }        
    }
}