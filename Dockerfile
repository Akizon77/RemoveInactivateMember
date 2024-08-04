FROM mcr.microsoft.com/dotnet/runtime:8.0

WORKDIR /app

COPY libe_sqlite3.so .
COPY DeleteInactiveMembers .
COPY .env .

RUN chmod +x ./DeleteInactiveMembers
ENTRYPOINT ["./DeleteInactiveMembers"]