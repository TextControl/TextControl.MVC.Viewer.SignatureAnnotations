using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using tx_signature_nofields.Models;

namespace tx_signature_nofields.Controllers {
	public class HomeController : Controller {
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger) {
			_logger = logger;
		}

		public IActionResult Index() {
			return View();
		}

      [HttpPost]
      public IActionResult MergeAnnotations([FromBody] List<List<Annotation>> annotations) {

			byte[] bTx;
			int iPageNumber = 0;

			// calculate the current resolution
			var dpi = 1440 / TXTextControl.DocumentServer.DocumentController.DpiX;

			// create temporary ServerTextControl
			using (TXTextControl.ServerTextControl tx = new TXTextControl.ServerTextControl()) {
            tx.Create();
				
            // load the document
            tx.Load("App_Data/template.tx", TXTextControl.StreamType.InternalUnicodeFormat);

				// loop through the array of annotations
				foreach (List<Annotation> pages in annotations) {

					iPageNumber++;

					foreach (Annotation annotation in pages) {
						
						// handle only signatures
						if (annotation.pen.type != "signature")
							continue;

						// get SVG as bytes and remove the first 24 characters
						// "data:image/svg+xml;utf8,"
						byte[] bytes = Encoding.UTF8.GetBytes(annotation.image.Remove(0, 24));

						// create a memory stream from SVG
						using (MemoryStream ms = new MemoryStream(
							bytes, 0, bytes.Length, writable: false, publiclyVisible: true)) {

							// TX image from memory stream
							TXTextControl.Image img = new TXTextControl.Image(ms);

							// add the image as a fixed object on the current page (array)
							tx.Images.Add(
								img,
								iPageNumber,
								new System.Drawing.Point(0, (int)(annotation.location.y * dpi)),
								TXTextControl.ImageInsertionMode.AboveTheText | TXTextControl.ImageInsertionMode.FixedOnPage);

							// set the location
							img.Location = new System.Drawing.Point(
									(int)(annotation.location.x * dpi),
									img.Location.Y);

						}

					}

				}

				// save the document as PDF
				tx.Save(out bTx, TXTextControl.BinaryStreamType.InternalUnicodeFormat);
         }

         // return as base64 encoded string
         return Ok(new Document() { Data = Convert.ToBase64String(bTx) });
      }

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error() {
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}