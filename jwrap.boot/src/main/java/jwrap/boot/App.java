/*
 * This Java source file was generated by the Gradle 'init' task.
 */
package jwrap.boot;

import java.io.File;
import java.lang.reflect.Field;
import java.lang.reflect.Method;
import java.net.URL;
import java.net.URLClassLoader;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.Arrays;
import java.util.Base64;

public class App {
	public static void main(String[] args) {
		//System.out.println("java.library.path=" + System.getProperty("java.library.path"));
		String jar = null;
		String main = null;
		String[] arguments = null;
		for (int i = 0; i < args.length; i++) {
			System.out.println(args[i]);
			switch (args[i]) {
			case "--jar":
				jar = parseStringArg(args[i + 1]);
				System.out.println(jar);
				i++;
				break;
			case "--main":
				main = parseStringArg(args[i + 1]);
				System.out.println(main);
				i++;
				break;
			case "--args":
				arguments = parseStringArrayArg(args[i + 1]);
				System.out.println(String.join(",", arguments));
				i++;
				break;
			}
		}
		System.out.println("new File(jar).exists() = " + new File(jar).exists());
		String parentDir = getParentDirPath(jar);

		try {
			URL url = (new File(jar)).toURI().toURL();
			URLClassLoader classLoader = URLClassLoader.newInstance(new URL[] { url });
			Class<?> globalMain = classLoader.loadClass(main);
			Method staticMethod = globalMain.getDeclaredMethod("main", String[].class);
			staticMethod.invoke(null, new Object[] { arguments });
		} catch (Exception ex) {
			ex.printStackTrace();
			System.exit(1);
		}
	}

	private static String parseStringArg(String base64) {
		byte[] decodedBytes = Base64.getDecoder().decode(base64);
		return new String(decodedBytes);
	}

	private static String[] parseStringArrayArg(String base64List) {
		String[] args = base64List.split(",");
		String[] result = new String[args.length];
		for (int i = 0; i < args.length; i++) {
			result[i] = parseStringArg(args[i]);
		}
		return result;
	}

	private static String getParentDirPath(String filePath) {
		Path path = Paths.get(filePath);
		Path parentPath = path.getParent();
		return parentPath.toString();
	}
}
