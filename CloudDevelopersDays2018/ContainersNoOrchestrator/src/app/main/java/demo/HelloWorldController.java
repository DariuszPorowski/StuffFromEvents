package demo;

import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.ResponseBody;
import org.springframework.web.bind.annotation.RequestParam;

@Controller
public class HelloWorldController {

	@GetMapping("/")
	@ResponseBody
	public String helloWorld(@RequestParam(value="name", required=false, defaultValue="World") String name) {
		return "Hello " + name;
	}

}
