# Summary
This application demonstrates some usages of native windows message calls to turn off all attached monitors that we believe are no longer working as expected. Our reproduction steps are not 100% reliable but we do observe the issue more than not.
* Start the test application
* Use any of the buttons on the left to sleep the device 
* Immediately attempt wiggling the mouse continuously
* The expectation is that the monitors will turn back on and you will be returned to desktop or the user login screen
* In reality we frequently observe that our monitors get stuck flickering
* This flickering will typically cease once input has stopped (i.e. the mouse is stopped) and the monitors can then be turned back on by one final input.

The BroadcastSystemMessage, SendMessage and PostMessage all exhibit this behaviour when provided (what we believe) are the correct flags and data to turn off the attached monitors.

## Reproduction notes
* We observe the issue on Windows 11 but no Windows 10. We believe this is a regression introduced in Windows but have not determined the exact version. 
* We have reproduced the issue on many device types. Multiple desktop computers, laptops and other custom surface based devices.

## Investigation notes
Our application uses the [BroadcastSystemMessage](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-broadcastsystemmessage) call in the test app and passes the same argument for each parameter. Copied from the linked web page these parameters are:

**Flags**: (BSF_FORCEIFHUNG) → Continues to broadcast the message, even if the time-out period elapses or one of the recipients is not responding.

**LPDWORD**: A pointer to a variable that contains and receives information about the recipients of the message. (BSM_APPLICATIONS) → Broadcast to applications. Also possible are BSM_ALLCOMPONENTS and BSM_ALLDESKTOPS

**Msg**: The type of message to be sent. In this case WM_SYSCOMMAND. “A window receives this message when the user chooses a command from the Window menu (formerly known as the system or control menu) or when the user chooses the maximize button, minimize button, restore button, or close button.”

**WPARAM**: For WM_SYSCOMMAND this means the type of system command requested. The four low-order bits of the wParam parameter are used internally by the system. Bits 4-15 contain a command code which can be a variety of windows operations (maximise/minimise etc) including SC_MONITORPOWER.

**LPARAM**: For WM_SYSCOMMAND. For SC_MONITORPOWER the lParam parameter can have the following values 
* -1 (the display is powering on)
* 1 (the display is going to low power)
* 2 (the display is being shut off)

# Background
We have a desktop application that is acts as an accessibiltity aid for our users. This has various integrations with native messaging and exotic input devices. One function the application provides is to turn the users monitors off. We had reports that this was not working as expected with users getting stuck with their screens flickering when the moni