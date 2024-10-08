FROM mcr.microsoft.com/dotnet/runtime:8.0

WORKDIR /app

COPY libe_sqlite3.so .
COPY DeleteInactiveMembers .

RUN chmod +x ./DeleteInactiveMembers
ENTRYPOINT ["./DeleteInactiveMembers"]
