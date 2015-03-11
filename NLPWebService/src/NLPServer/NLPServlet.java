package NLPServer;

import java.io.BufferedOutputStream;
import java.io.BufferedWriter;
import java.io.IOException;
import java.io.OutputStreamWriter;
import java.io.UnsupportedEncodingException;
import java.net.URLDecoder;
import java.net.URLEncoder;
import java.util.logging.Level;
import java.util.logging.Logger;

import javax.servlet.ServletException;
import javax.servlet.annotation.WebServlet;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.apache.http.client.utils.URLEncodedUtils;

/**
 * Servlet implementation class NLPServlet
 */
@WebServlet("/NLPServlet")
public class NLPServlet extends HttpServlet {
	private static final long serialVersionUID = 1L;

	NLPCore nlpcore = new NLPCore();
	
    /**
     * Default constructor. 
     */
    public NLPServlet() {
        // TODO Auto-generated constructor stub
    }

	/**
	 * @see HttpServlet#doGet(HttpServletRequest request, HttpServletResponse response)
	 */
	protected void doGet(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
		request.setCharacterEncoding("UTF-8");
		String rawWords = request.getParameter("w");
		String res = "对不起，麻烦您重说一遍";
        try {
            res = nlpcore.ReturnMessage(rawWords);
        } catch (Exception ex) {
            System.out.println("Hello......");
            Logger.getLogger(NLPWebService.class.getName()).log(Level.SEVERE, null, ex);
        }
        
        BufferedWriter bw = new BufferedWriter(new OutputStreamWriter(response.getOutputStream()));
        bw.write(String.format("{\"ret\":%1$s}", res));
        bw.flush();
        bw.close();
	}

	/**
	 * @see HttpServlet#doPost(HttpServletRequest request, HttpServletResponse response)
	 */
	protected void doPost(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
		doGet(request, response);
	}

}
