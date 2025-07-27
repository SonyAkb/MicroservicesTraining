# Микросервисы на C# (.NET 8)

Language
- [English](Microservices-in-C#-(.NET-8):-Order-Service-and-Product-Service)
- [Русский](#Микросервисы-на-C#-(.NET-8):-Order-Service-и-Product-Service)

---

---

---
  
# Microservices in C# (.NET 8): Order Service and Product Service

## Table of Contents
1. [Project Description](#project-description)  
2. [Technologies and Tools](#technologies-and-tools)  
3. [Architecture](#architecture)  
4. [Project Setup](#project-setup)  
5. [API Endpoints](#api-endpoints)  
6. [Request Examples](#request-examples)  
7. [Issues and Solutions](#issues-and-solutions)  
8. [Future Improvements](#future-improvements)  

---

## Project Description
The project consists of two microservices:
- **Order Service**: A service for creating and managing orders.  
- **Product Service**: A service for managing product information and stock availability.  

The Order Service communicates with the Product Service to check product availability before creating an order.  

---

## Technologies and Tools
- **Language**: C# (.NET 8)  
- **Communication Protocol**: gRPC  
- **Database**: In-Memory  
- **API Documentation**: Swagger  

---

## Architecture
1. **Product Service**:
   - Stores product data (name, description, stock quantity).  
   - Provides product information upon request.  

2. **Order Service**:
   - Creates orders after verifying product availability via Product Service.  
   - Reserves products when creating orders.  

The services communicate via gRPC.  

---

## Project Setup
1. **Requirements**:
   - Installed .NET 8 SDK.  

2. **Instructions**:
   - Clone the repository:  
     ```bash
     git clone https://github.com/SonyAkb/MicroservicesTraining.git
     ```
   - Run each service separately:  
     ```bash
     cd ProductService
     dotnet run
     ```
     ```bash
     cd OrderService
     dotnet run
     ```

---

## API Endpoints
### Product Service
- `GET /products/{id}` — Get product information.  
- `POST /products` — Add a new product.  
- `PUT /products/{id}/stock` — Update product stock quantity.  

### Order Service
- `POST /orders` — Create an order (checks product availability).  
- `GET /orders/{id}` — Get order information.  

---

## Request Examples
### Product Service
#### Get All Products
<img width="427" height="762" alt="image" src="https://github.com/user-attachments/assets/3a27e4a0-3c2e-4153-ba74-3ba2b1cfc5ac" />

#### Add New Product
<img width="178" height="150" alt="image" src="https://github.com/user-attachments/assets/60b95e4d-a92e-46e7-a2bc-02ff3daf0e51" />
<img width="388" height="594" alt="image" src="https://github.com/user-attachments/assets/24288e66-dc2c-4b96-a59c-d96211b1b14d" />

#### Get Product by ID
<img width="387" height="774" alt="image" src="https://github.com/user-attachments/assets/9a022f06-4620-491f-a18e-8b124b09f820" />

#### Update Product Stock by ID
<img width="367" height="716" alt="image" src="https://github.com/user-attachments/assets/5da33cf0-0df8-4692-8a92-1a65332b48ad" />

### Order Service
#### Get All Orders
<img width="391" height="539" alt="image" src="https://github.com/user-attachments/assets/8840f3c2-9ee5-450f-88f4-7e84cbb2178a" />

#### Create Order
<img width="297" height="157" alt="image" src="https://github.com/user-attachments/assets/e1331de0-dc95-4a1a-9a02-27c48da8b6b3" />
<img width="372" height="598" alt="image" src="https://github.com/user-attachments/assets/6678650c-6887-48e0-a18a-a8aec42e3de1" />

#### Get Order by ID
<img width="422" height="99" alt="image" src="https://github.com/user-attachments/assets/6d7380dd-4c7a-49c4-a310-946ebb83f9c0" />
<img width="372" height="507" alt="image" src="https://github.com/user-attachments/assets/94dc2b23-a774-451f-84de-f18d951ea716" />

---

## Issues and Solutions
**Issue**: Communication between services doesn't work.  
**Solution**: Verify gRPC/REST URLs and ports in configuration files.  

**Issue**: Errors during product reservation.  
**Solution**: Ensure Product Service returns correct stock availability data.  

---

## Future Improvements
- Implement JWT authentication  
- Add Docker containerization  
- Write unit tests  
- Implement RabbitMQ messaging  

---

---

# Микросервисы на C# (.NET 8): Order Service и Product Service
  
## Содержание
1. [Описание проекта](#описание-проекта)  
2. [Технологии и инструменты](#технологии-и-инструменты)  
3. [Архитектура](#архитектура)  
4. [Запуск проекта](#запуск-проекта)  
5. [API Endpoints](#api-endpoints)  
6. [Примеры запросов](#примеры-запросов)   
7. [Проблемы и решения](#проблемы-и-решения)  
8. [Планы по доработке](#планы-по-доработке)   

---

## Описание проекта
Проект представляет собой два микросервиса:
- **Order Service**: сервис для создания и управления заказами.  
- **Product Service**: сервис для управления информацией о продуктах и их наличием на складе.  

Order Service взаимодействует с Product Service для проверки наличия товара перед созданием заказа.  

---

## Технологии и инструменты
- **Язык**: C# (.NET 8)  
- **Формат коммуникации**: gRPC  
- **База данных**: In-Memory 
- **Документирование API**: Swagger      

---

## Архитектура
1. **Product Service**:
   - Хранение данных о продуктах (название, описание, количество на складе).  
   - Предоставление информации о продуктах по запросу.  

2. **Order Service**:
   - Создание заказов с проверкой наличия товара через Product Service.  
   - Резервирование товара при создании заказа.  

Связь между сервисами реализована через gRPC.  

---

## Запуск проекта
1. **Требования**:
   - Установленный .NET 8 SDK.  

2. **Инструкции**:
   - Клонируйте репозиторий:  
     ```bash
     git clone https://github.com/SonyAkb/MicroservicesTraining.git
     ```
   - Запустите каждый сервис отдельно:  
     ```bash
     cd ProductService
     dotnet run
     ```
     ```bash
     cd OrderService
     dotnet run
     ```

---

## API Endpoints
### Product Service
- `GET /products/{id}` — получить информацию о продукте.  
- `POST /products` — добавить новый продукт.  
- `PUT /products/{id}/stock` — изменить количество товара на складе.  

### Order Service
- `POST /orders` — создать заказ (проверяет наличие товара).  
- `GET /orders/{id}` — получить информацию о заказе.  

---

## Примеры запросов
### Product Service
#### Получение всех продуктов
<img width="427" height="762" alt="image" src="https://github.com/user-attachments/assets/3a27e4a0-3c2e-4153-ba74-3ba2b1cfc5ac" />

#### Добавление нового продукта

<img width="178" height="150" alt="image" src="https://github.com/user-attachments/assets/60b95e4d-a92e-46e7-a2bc-02ff3daf0e51" />

<img width="388" height="594" alt="image" src="https://github.com/user-attachments/assets/24288e66-dc2c-4b96-a59c-d96211b1b14d" />

#### Получение продукта по ID

<img width="387" height="774" alt="image" src="https://github.com/user-attachments/assets/9a022f06-4620-491f-a18e-8b124b09f820" />

#### Изменение количества запасов продукта по ID

<img width="367" height="716" alt="image" src="https://github.com/user-attachments/assets/5da33cf0-0df8-4692-8a92-1a65332b48ad" />

### Order Service

#### Вывод всех заказов

<img width="391" height="539" alt="image" src="https://github.com/user-attachments/assets/8840f3c2-9ee5-450f-88f4-7e84cbb2178a" />

#### Создание заказа

<img width="297" height="157" alt="image" src="https://github.com/user-attachments/assets/e1331de0-dc95-4a1a-9a02-27c48da8b6b3" />

<img width="372" height="598" alt="image" src="https://github.com/user-attachments/assets/6678650c-6887-48e0-a18a-a8aec42e3de1" />

#### Поиск заказа по ID

<img width="422" height="99" alt="image" src="https://github.com/user-attachments/assets/6d7380dd-4c7a-49c4-a310-946ebb83f9c0" />

<img width="372" height="507" alt="image" src="https://github.com/user-attachments/assets/94dc2b23-a774-451f-84de-f18d951ea716" />

---

## Проблемы и решения

Проблема: Не работает взаимодействие между сервисами.

Решение: Проверить корректность URL и портов в настройках gRPC/REST.


Проблема: Ошибки при резервировании товара.

Решение: Убедиться, что Product Service возвращает корректные данные о наличии товара.

---

## Планы по доработке

- Добавить аутентификацию через JWT
- Реализовать контейнеризацию через Docker.
- Написать  unit-тесты.
- Реализовать использование RabbitMQ.
