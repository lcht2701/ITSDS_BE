using AutoWrapper;

namespace API.Configurations;

public static class ApplicationConfig
{
    public static WebApplication UseAutoWrapper(this WebApplication app)
    {
        app.UseApiResponseAndExceptionWrapper(
            new AutoWrapperOptions
            {
                IsApiOnly = false,
                ShowIsErrorFlagForSuccessfulResponse = true,
                WrapWhenApiPathStartsWith = "/v1/itsds"
            }
        );
        return app;
    }
}
