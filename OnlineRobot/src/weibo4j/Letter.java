package weibo4j;

import java.util.List;
import java.util.Map;

import weibo4j.model.Comment;
import weibo4j.model.CommentWapper;
import weibo4j.model.Paging;
import weibo4j.model.PostParameter;
import weibo4j.model.WeiboException;
import weibo4j.util.ArrayUtils;
import weibo4j.util.WeiboConfig;

import weibo4j.model.DirectMessage;

public class Letter extends Weibo{
	private static final long serialVersionUID = 3321231200237418256L;

	public Letter(String access_token) {
		this.access_token = access_token;
	}
//	public DirectMessage sendDirectMessage(String id,
//            String text) throws WeiboException {
//			return new DirectMessage(client.post(WeiboConfig.getValue("baseURL") + "messages/reply.json",
//					new PostParameter[]{new PostParameter("receiver_id", id),
//					new PostParameter("data", text)}, 
//					access_token));
//}
}
