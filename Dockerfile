# ---------- Build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Directory.Build.props Nega-com.sln ./
COPY BE/BE.csproj BE/
COPY DAL/DAL.csproj DAL/
COPY BLL/BLL.csproj BLL/
COPY Nega.com/Negacom.csproj Nega.com/
RUN dotnet restore Nega.com/Negacom.csproj
COPY . .
RUN dotnet publish Nega.com/Negacom.csproj -c Release -o /app --no-restore

# ---------- Run ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app .
# SQLite database + uploads must be writable
RUN mkdir -p /app/App_Data /app/wwwroot/uploads/content && chown -R app:app /app/App_Data /app/wwwroot/uploads
USER app
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    Demo__Enabled=true \
    Demo__ResetMinutes=60
EXPOSE 8080
ENTRYPOINT ["dotnet", "Negacom.dll"]
