/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package NLPServer;

import java.util.logging.Level;
import java.util.logging.Logger;
import javax.jws.WebService;
import javax.jws.WebMethod;
import javax.jws.WebParam;

/**
 *
 * @author v-feiche
 */
@WebService(serviceName = "NLPWebService")
public class NLPWebService {
 
    NLPCore nlpcore;
    
    
    public NLPWebService() {
        this.nlpcore = new NLPCore();
        
        //nlpcore.InitNLPCore();
    }
    
    @WebMethod(operationName = "hello")
    public String hello(@WebParam(name = "name") String txt) {
        return "Hello " + txt + " !";
    }
    @WebMethod(operationName = "DoNLP")
    public String DoNLP(@WebParam(name = "words") String words) {
        String res = "对不起，麻烦您重说一遍";
        try {
            System.out.println(res);
            res = nlpcore.ReturnMessage(words);
            //msq.Send(res);
            //msq.Send(r***&&es);
            //msq.Send("Hello!!!!!!");
            //res = "hahah";
            //System.out.println(res);
        } catch (Exception ex) {
            System.out.println("Hello......");
            Logger.getLogger(NLPWebService.class.getName()).log(Level.SEVERE, null, ex);
        }
        return res;
    }
}
