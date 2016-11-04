Restfull WebApi
=======================

Story:
1) As a Manager I want to be able to create, read, update or delete the Document; 
  - Only owner can update or delete the Document;
  - Cascade delete behavior should be in place;
  
2) As an Employee I want to be able to read the Document;
3) As a Manager or Employee I want to be able to create, read, update or delete the Comment to the Document;
  - Only owner can updated or delete the Comment;
 
4) Tech story: For the debugging purpose application should emit reasonable amount of log records;
  
The implemented application should be exposed as a set of RESTful endpoints that cover at least the following behavior:
 - to CRUD the User with two roles Employee and Manager;
 - to CRUD the Document;
 - to CRUD the Comment;
  
Desired aspects are to be covered:
1) Technical stack: WebAPI, OWIN, EntityFramework, MS-SQL
3) Integration test coverage
4) Continuous Integration
5) Cross-cutting for logging behavior