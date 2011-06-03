INSTRUCTIONS
============
1. Right click on the dk.local.pfx and select Install PFX.

2. [Password]
	Password is dk 
	Leave other items unchecked.


3. [Certificate Store]
	Automatically select 

4. When complete, open Certificates.msc.
5. Move DropkicKLocal from "Certificates - Current User \ Personal \ Certificates" to the "Certificates (Local Computer) \ Personal" store.
6. It should create a folder named certificates if not already there. 
7. That's it.



DETAILS
=======
This is based on http://compilewith.net/2007/12/creating-test-x509-certificates.html
Open Visual Studio Command line and type:
MakeCert -r -pe -n "CN=DropkicKLocal" -sky exchange -ss My -sr LocalMachine