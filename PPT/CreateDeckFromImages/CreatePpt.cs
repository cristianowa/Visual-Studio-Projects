using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using a = DocumentFormat.OpenXml.Drawing;

class Program
{
  static void Main(string[] args)
  {
    string newPresentation = "DeckFromImages.pptx";
    string presentationTemplate = "PresentationTemplate.pptx";
    string presentationFolder = @"C:\Temp\";
    string imageFolder = @"C:\Dev\Work\Akona\Office 2007 VHTs\Topics\Phase 2\Creating PPT Presentation From Folder of Images\Code\Images";
    string[] imageFileExtensions = new[] { "*.jpg", "*.jpeg", "*.gif" };

    // Make a copy of the template presentation. This will throw an exception if the template 
    // presentation does not exist.
    File.Copy(presentationFolder + presentationTemplate, presentationFolder + newPresentation, true);

    // Get the image files in the image folder.
    List<string> imageFileNames = GetImageFileNames(imageFolder, imageFileExtensions);

    // Create new slides for the images.
    if (imageFileNames.Count() > 0)
      CreateSlides(imageFileNames, presentationFolder + newPresentation);

    // Validate the new presentation.
    OpenXmlValidator validator = new OpenXmlValidator();

    var errors = validator.Validate(presentationFolder + newPresentation);

    if (errors.Count() > 0)
    {
      Console.WriteLine("The deck creation process completed but the created presentation failed to validate.");
      Console.WriteLine("There are " + errors.Count() + " errors:\r\n");

      DisplayValidationErrors(errors);
    }
    else
      Console.WriteLine("The deck creation process completed and the created presentation validated with 0 errors.");
  }

  static void CreateSlides(List<string> imageFileNames, string newPresentation)
  {
    int id = 0;
    string relId;
    SlideId newSlideId;
    SlideLayoutId newSlideLayoutId;

    string imageFileNameNoPath;

    long imageWidthEMU = 0;
    long imageHeightEMU = 0;

    // Open the new presentation.
    using (PresentationDocument newDeck = PresentationDocument.Open(newPresentation, true))
    {
      // Get the presentation part of the new deck.
      PresentationPart presentationPart = newDeck.PresentationPart;

      // Reuse the slide master. Otherwise, create a new slide master part and a new theme part.
      var slideMasterPart = presentationPart.SlideMasterParts.First();

      // If the new presentation doesn't have a SlideIdList element yet then add it.
      if (presentationPart.Presentation.SlideIdList == null)
        presentationPart.Presentation.SlideIdList = new SlideIdList();

      // If the slide master doesn't have a SlideLayoutIdList element yet then add it.
      if (slideMasterPart.SlideMaster.SlideLayoutIdList == null)
          slideMasterPart.SlideMaster.SlideLayoutIdList = new SlideLayoutIdList();
      
      // Get a unique id for both the slide master id and slide layout id lists.
      uint uniqueId = GetMaxUniqueId(presentationPart);
      
      // Get a unique id for the slide id list.
      uint maxSlideId = GetMaxSlideId(presentationPart.Presentation.SlideIdList);

      // Loop through each file in the image folder creating slides in the new presentation.
      foreach (string imageFileNameWithPath in imageFileNames)
      {
        imageFileNameNoPath = Path.GetFileNameWithoutExtension(imageFileNameWithPath);

        // Create a unique relationship id based on the name of the image file.
        id++;
        relId = imageFileNameNoPath.Replace(" ", "") + id;

        // Get the bytes and size of the image.
        byte[] imageBytes = GetImageData(imageFileNameWithPath, ref imageWidthEMU, ref imageHeightEMU);

        // Create the new slide part.
        var slidePart = presentationPart.AddNewPart<SlidePart>(relId);
        GenerateSlidePart(relId, imageFileNameNoPath, imageFileNameNoPath, imageWidthEMU, imageHeightEMU).Save(slidePart);

        var imagePart = slidePart.AddImagePart(ImagePartType.Jpeg, relId);
        GenerateImagePart(imagePart, imageBytes);

        //slidePart.AddPart(slideLayoutPart);
        var slideLayoutPart = slidePart.AddNewPart<SlideLayoutPart>();
        GenerateSlideLayoutPart().Save(slideLayoutPart);       

        slideMasterPart.AddPart(slideLayoutPart);
        slideLayoutPart.AddPart(slideMasterPart);

        // Add new slide layout into the list in slideMasterPart
        uniqueId++;
        newSlideLayoutId = new SlideLayoutId();
        newSlideLayoutId.RelationshipId = slideMasterPart.GetIdOfPart(slideLayoutPart);
        newSlideLayoutId.Id = uniqueId;
        slideMasterPart.SlideMaster.SlideLayoutIdList.Append(newSlideLayoutId);

        // Add slide to slide list.
        maxSlideId++;
        newSlideId = new SlideId();
        newSlideId.RelationshipId = relId;
        newSlideId.Id = maxSlideId;
        presentationPart.Presentation.SlideIdList.Append(newSlideId);
      }

      slideMasterPart.SlideMaster.Save();

      // Make sure all slide ids are unique.
      // FixSlideLayoutIds(presentationPart);

      // Save the changes to the new deck.
      presentationPart.Presentation.Save();
    }
  }

  public static List<string> GetImageFileNames(string imageFolder, string[] imageFileExtensions)
  {
    // Create a list to hold the names of the files with the requested extensions.
    List<string> fileNames = new List<string>();

    // Loop through each file extension.
    foreach (string extension in imageFileExtensions)
    {
      // Add all the files that match the current extension to the list of file names.
      fileNames.AddRange(Directory.GetFiles(imageFolder, extension, SearchOption.TopDirectoryOnly));
    }

    // Return the list of file names.
    return fileNames;
  }

  static uint GetMaxSlideId(SlideIdList slideIdList)
  {
    // Slide identifiers have a minimum value of greater than or equal to 256
    // and a maximum value of less than 2147483648. 
    uint max = 256;

    if (slideIdList != null)
      // Get the maximum id value from the current set of children.
      foreach (SlideId child in slideIdList.Elements<SlideId>())
      {
        uint id = child.Id;

        if (id > max)
          max = id;
      }

    return max;
  }

  static uint GetMaxUniqueId(PresentationPart presentationPart)
  {
      // Slide master identifiers have a minimum value of greater than or equal to 2147483648. 
      uint max = 2147483648;

      var slideMasterIdList = presentationPart.Presentation.SlideMasterIdList;

      if (slideMasterIdList != null)
          // Get the maximum id value from the current set of children.
          foreach (SlideMasterId child in slideMasterIdList.Elements<SlideMasterId>())
          {
              uint id = child.Id;

              if (id > max)
                  max = id;
          }

      foreach (var slideMasterPart in presentationPart.SlideMasterParts)
      {
          var slideLayoutIdList = slideMasterPart.SlideMaster.SlideLayoutIdList;
          if (slideLayoutIdList != null)
              // Get the maximum id value from the current set of children.
              foreach (var child in slideLayoutIdList.Elements<SlideLayoutId>())
              {
                  uint id = child.Id;

                  if (id > max)
                      max = id;
              }
      }

      return max;
  }

  private static Slide GenerateSlidePart(string imagePartId, string imageName, string imageDescription, long imageWidthEMU, long imageHeightEMU)
  {
    var element =
        new Slide(
            new CommonSlideData(
                new ShapeTree(
                    new NonVisualGroupShapeProperties(
                        new NonVisualDrawingProperties() { Id = (UInt32Value)1U, Name = "" },
                        new NonVisualGroupShapeDrawingProperties(),
                        new AppNonVisualDrawingProperties()),
                    new GroupShapeProperties(
                        new a.TransformGroup(
                            new a.Offset() { X = 0L, Y = 0L },
                            new a.Extents() { Cx = 0L, Cy = 0L },
                            new a.ChildOffset() { X = 0L, Y = 0L },
                            new a.ChildExtents() { Cx = 0L, Cy = 0L })),
                    new Picture(
                        new NonVisualPictureProperties(
                            new NonVisualDrawingProperties() { Id = (UInt32Value)4U, Name = imageName, Description = imageDescription },
                            new NonVisualPictureDrawingProperties(
                                new a.PictureLocks() { NoChangeAspect = true }),
                            new AppNonVisualDrawingProperties()),
                        new BlipFillProperties(
                            new a.Blip() { Embed = imagePartId },
                            new a.Stretch(
                                new a.FillRectangle())),
                        new ShapeProperties(
                            new a.Transform2D(
                                new a.Offset() { X = 0L, Y = 0L },
                                new a.Extents() { Cx = imageWidthEMU, Cy = imageHeightEMU }),
                            new a.PresetGeometry(
                                new a.AdjustValueList()
                            ) { Preset = a.ShapeTypeValues.Rectangle })))),
            new ColorMapOverride(
                new a.MasterColorMapping()));

    return element;
  }

  private static byte[] GetImageData(string imageFilePath, ref long imageWidthEMU, ref long imageHeightEMU)
  {
    byte[] imageFileBytes;
    Bitmap imageFile;

    // Open a stream on the image file and read it's contents. The
    // following code will generate an exception if an invalid file
    // name is passed.
    using (FileStream fsImageFile = File.OpenRead(imageFilePath))
    {
      imageFileBytes = new byte[fsImageFile.Length];
      fsImageFile.Read(imageFileBytes, 0, imageFileBytes.Length);

      imageFile = new Bitmap(fsImageFile);
    }

    // Get the dimensions of the image in English Metric Units (EMU)
    // for use when adding the markup for the image to the document.
    imageWidthEMU =
        (long)((imageFile.Width / imageFile.HorizontalResolution) * 914400L);
    imageHeightEMU =
        (long)((imageFile.Height / imageFile.VerticalResolution) * 914400L);

    return imageFileBytes;
  }

  private static void GenerateImagePart(OpenXmlPart part, byte[] imageFileBytes)
  {
    // Write the contents of the image to the ImagePart.
    using (BinaryWriter writer = new BinaryWriter(part.GetStream()))
    {
      writer.Write(imageFileBytes);
      writer.Flush();
    }
  }

  private static SlideLayout GenerateSlideLayoutPart()
  {
    var element =
        new SlideLayout(
            new CommonSlideData(
                new ShapeTree(
                    new NonVisualGroupShapeProperties(
                        new NonVisualDrawingProperties() { Id = (UInt32Value)1U, Name = "" },
                        new NonVisualGroupShapeDrawingProperties(),
                        new AppNonVisualDrawingProperties()),
                    new GroupShapeProperties(
                        new a.TransformGroup(
                            new a.Offset() { X = 0L, Y = 0L },
                            new a.Extents() { Cx = 0L, Cy = 0L },
                            new a.ChildOffset() { X = 0L, Y = 0L },
                            new a.ChildExtents() { Cx = 0L, Cy = 0L })))
            ) { Name = "Title Slide" },
            new ColorMapOverride(
                new a.MasterColorMapping())
        ) { Type = SlideLayoutValues.Title, Preserve = true };

    return element;
  }

    static void DisplayValidationErrors(IEnumerable<ValidationErrorInfo> errors)
  {
    int errorIndex = 1;

    foreach (ValidationErrorInfo errorInfo in errors)
    {
      Console.WriteLine(errorInfo.Description);
      Console.WriteLine(errorInfo.Path.XPath);

      if (++errorIndex <= errors.Count())
        Console.WriteLine("================");
    }
  }
}
