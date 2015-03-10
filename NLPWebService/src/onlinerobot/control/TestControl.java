package onlinerobot.control;


public class TestControl {

	/**
	 * @param args
	 */
	public static void main(String[] args) {
		// TODO Auto-generated method stub
		ControlService control = new ControlService();
		ControlDelegate ControlDelegate = control.getControlPort();
		//List<String> args = null;
		//String fromRobot = ControlDelegate.getSobot("股票，中泰化学");
		//String fromBing = ControlDelegate.getBing("北京");
		String fromTuLing = ControlDelegate.getTuLing("鱼香肉丝");
		//System.out.println("fromRobot:" + fromRobot);
		//System.out.println("fromBing:" + fromBing);
		System.out.println("fromTuLing:" + fromTuLing);
	}

}
