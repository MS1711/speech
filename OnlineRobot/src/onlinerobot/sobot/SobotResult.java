package onlinerobot.sobot;

import java.util.List;

public class SobotResult {
	private int status;
	private String message;
	private String userId;
	private String content;
	private List<String> contents;
	public String getContent() {
		return content;
	}
	public List<String> getContents() {
		return contents;
	}
	public String getMessage() {
		return message;
	}
	public int getStatus() {
		return status;
	}
	public String getUserId() {
		return userId;
	}
	public void setContent(String content) {
		this.content = content;
	}
	public void setContents(List<String> contents) {
		this.contents = contents;
	}
	public void setMessage(String message) {
		this.message = message;
	}
	public void setStatus(int status) {
		this.status = status;
	}
	public void setUserId(String userId) {
		this.userId = userId;
	}
}
