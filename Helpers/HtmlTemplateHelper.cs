
public static class HtmlTemplateHelper
{


    /// <summary>
    /// Returns a common set of CSS styles used for formatting HTML email templates.
    /// Designed to replicate Bootstrap utility classes for compatibility in email clients.
    /// </summary>
    public static string GenerateCSSCommonStyles()
    {
        return @"
<style>
  body {
    background-color: #f8f9fa !important;
    background-color: #f8f9fa !important;
    margin: 0;
    padding: 0;
    font-family: 'Trebuchet MS', 'Helvetica', 'Arial', sans-serif;
    font-size: 1rem;
  }

  .container {
    margin: 3rem auto;
    padding-left: 1rem;
    padding-right: 1rem;
    max-width: 650px;
  }

  .bg-white {
    background-color: #ffffff;
  }

  .p-4 {
    padding: 1.5rem;
  }

  .rounded {
    border-radius: 0.5rem;
  }

  .shadow-sm {
    box-shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.075) !important;
  }

  .text-start {
    text-align: left;
  }

  .mb-2 {
    margin-bottom: 0.5rem;
  }

  .mb-4 {
    margin-bottom: 1.5rem;
  }

  .h4 {
    font-size: 1.25rem;
    margin: 0;
  }

  .text-dark {
    color: #212529;
  }

  .text-secondary {
    color: #6c757d;
    margin: 0 0 1rem;
  }

  .btn {
    display: inline-block;
    font-weight: 400;
    color: #fff !important;
    text-align: center;
    text-decoration: none;
    background-color: #212529;
    border: 1px solid #212529;
    padding: 0.5rem 1rem;
    font-size: 1rem;
    border-radius: 0.375rem;
  }

  .text-muted {
    color: #6c757d;
  }

  .mt-3 {
    margin-top: 1rem;
  }

  .small {
    font-size: 0.8rem;
  }

  .img-fluid {
    max-width: 100%;
    height: auto;
  }
  .me-2 {
        margin-right: 0.5rem;
      }

      .fw-semibold {
        font-weight: 600;
      }
      .me-1 {
        margin-right: 0.25rem;
      }


</style>
";
    }
}

