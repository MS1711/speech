package onlinerobot.control;

import javax.xml.bind.JAXBElement;
import javax.xml.bind.annotation.XmlElementDecl;
import javax.xml.bind.annotation.XmlRegistry;
import javax.xml.namespace.QName;

/**
 * This object contains factory methods for each Java content interface and Java
 * element interface generated in the onlinerobot.control package.
 * <p>
 * An ObjectFactory allows you to programatically construct new instances of the
 * Java representation for XML content. The Java representation of XML content
 * can consist of schema derived interfaces and classes representing the binding
 * of schema type definitions, element declarations and model groups. Factory
 * methods for each of these are provided in this class.
 * 
 */
@XmlRegistry
public class ObjectFactory {

	private final static QName _GetSobotResponse_QNAME = new QName(
			"http://control.onlinerobot/", "GetSobotResponse");
	private final static QName _Main_QNAME = new QName(
			"http://control.onlinerobot/", "main");
	private final static QName _GetTuLingResponse_QNAME = new QName(
			"http://control.onlinerobot/", "GetTuLingResponse");
	private final static QName _GetBing_QNAME = new QName(
			"http://control.onlinerobot/", "GetBing");
	private final static QName _MainResponse_QNAME = new QName(
			"http://control.onlinerobot/", "mainResponse");
	private final static QName _GetSobot_QNAME = new QName(
			"http://control.onlinerobot/", "GetSobot");
	private final static QName _GetBingResponse_QNAME = new QName(
			"http://control.onlinerobot/", "GetBingResponse");
	private final static QName _GetTuLing_QNAME = new QName(
			"http://control.onlinerobot/", "GetTuLing");

	/**
	 * Create a new ObjectFactory that can be used to create new instances of
	 * schema derived classes for package: onlinerobot.control
	 * 
	 */
	public ObjectFactory() {
	}

	/**
	 * Create an instance of {@link GetBing }
	 * 
	 */
	public GetBing createGetBing() {
		return new GetBing();
	}

	/**
	 * Create an instance of {@link GetSobotResponse }
	 * 
	 */
	public GetSobotResponse createGetSobotResponse() {
		return new GetSobotResponse();
	}

	/**
	 * Create an instance of {@link GetSobot }
	 * 
	 */
	public GetSobot createGetSobot() {
		return new GetSobot();
	}

	/**
	 * Create an instance of {@link GetTuLing }
	 * 
	 */
	public GetTuLing createGetTuLing() {
		return new GetTuLing();
	}

	/**
	 * Create an instance of {@link MainResponse }
	 * 
	 */
	public MainResponse createMainResponse() {
		return new MainResponse();
	}

	/**
	 * Create an instance of {@link Main }
	 * 
	 */
	public Main createMain() {
		return new Main();
	}

	/**
	 * Create an instance of {@link GetTuLingResponse }
	 * 
	 */
	public GetTuLingResponse createGetTuLingResponse() {
		return new GetTuLingResponse();
	}

	/**
	 * Create an instance of {@link GetBingResponse }
	 * 
	 */
	public GetBingResponse createGetBingResponse() {
		return new GetBingResponse();
	}

	/**
	 * Create an instance of {@link JAXBElement }{@code <}
	 * {@link GetSobotResponse }{@code >}
	 * 
	 */
	@XmlElementDecl(namespace = "http://control.onlinerobot/", name = "GetSobotResponse")
	public JAXBElement<GetSobotResponse> createGetSobotResponse(
			GetSobotResponse value) {
		return new JAXBElement<GetSobotResponse>(_GetSobotResponse_QNAME,
				GetSobotResponse.class, null, value);
	}

	/**
	 * Create an instance of {@link JAXBElement }{@code <}{@link Main }{@code >}
	 * 
	 */
	@XmlElementDecl(namespace = "http://control.onlinerobot/", name = "main")
	public JAXBElement<Main> createMain(Main value) {
		return new JAXBElement<Main>(_Main_QNAME, Main.class, null, value);
	}

	/**
	 * Create an instance of {@link JAXBElement }{@code <}
	 * {@link GetTuLingResponse }{@code >}
	 * 
	 */
	@XmlElementDecl(namespace = "http://control.onlinerobot/", name = "GetTuLingResponse")
	public JAXBElement<GetTuLingResponse> createGetTuLingResponse(
			GetTuLingResponse value) {
		return new JAXBElement<GetTuLingResponse>(_GetTuLingResponse_QNAME,
				GetTuLingResponse.class, null, value);
	}

	/**
	 * Create an instance of {@link JAXBElement }{@code <}{@link GetBing }{@code >}
	 * 
	 */
	@XmlElementDecl(namespace = "http://control.onlinerobot/", name = "GetBing")
	public JAXBElement<GetBing> createGetBing(GetBing value) {
		return new JAXBElement<GetBing>(_GetBing_QNAME, GetBing.class, null,
				value);
	}

	/**
	 * Create an instance of {@link JAXBElement }{@code <}{@link MainResponse }
	 * {@code >}
	 * 
	 */
	@XmlElementDecl(namespace = "http://control.onlinerobot/", name = "mainResponse")
	public JAXBElement<MainResponse> createMainResponse(MainResponse value) {
		return new JAXBElement<MainResponse>(_MainResponse_QNAME,
				MainResponse.class, null, value);
	}

	/**
	 * Create an instance of {@link JAXBElement }{@code <}{@link GetSobot }
	 * {@code >}
	 * 
	 */
	@XmlElementDecl(namespace = "http://control.onlinerobot/", name = "GetSobot")
	public JAXBElement<GetSobot> createGetSobot(GetSobot value) {
		return new JAXBElement<GetSobot>(_GetSobot_QNAME, GetSobot.class, null,
				value);
	}

	/**
	 * Create an instance of {@link JAXBElement }{@code <}{@link GetBingResponse }
	 * {@code >}
	 * 
	 */
	@XmlElementDecl(namespace = "http://control.onlinerobot/", name = "GetBingResponse")
	public JAXBElement<GetBingResponse> createGetBingResponse(
			GetBingResponse value) {
		return new JAXBElement<GetBingResponse>(_GetBingResponse_QNAME,
				GetBingResponse.class, null, value);
	}

	/**
	 * Create an instance of {@link JAXBElement }{@code <}{@link GetTuLing }
	 * {@code >}
	 * 
	 */
	@XmlElementDecl(namespace = "http://control.onlinerobot/", name = "GetTuLing")
	public JAXBElement<GetTuLing> createGetTuLing(GetTuLing value) {
		return new JAXBElement<GetTuLing>(_GetTuLing_QNAME, GetTuLing.class,
				null, value);
	}

}
