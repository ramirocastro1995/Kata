# Wallet Kata

Este proyecto se centra en la implementación de una API para gestionar cuentas y transacciones en una billetera digital. 

## Funcionalidades


# Wallet Endpoints

Este controlador se encarga de manejar las operaciones relacionadas con las cuentas de usuario (`Wallets`) y las transacciones (`Transactions`) dentro de la API.

## Rutas

### `GET /api/wallet/GetWallets`

Obtiene un listado de todas las cuentas (`Wallets`) con la posibilidad de aplicar filtros opcionales.

#### Parámetros de Consulta (Query Parameters):

- `userId` (int, opcional): ID del usuario. Si se proporciona, se devuelve solo la cuenta asociada a este ID.
- `userDocument` (string, opcional): Documento del usuario. Si se proporciona, se filtran las cuentas que coincidan con este documento.

#### Respuestas:

- `200 OK`: Devuelve un listado de cuentas (`WalletDto`) que cumplen con los filtros aplicados.
- `204 No Content`: No se encontraron cuentas que cumplan con los filtros especificados.
- `404 Not Found`: Si se especifica `userId` y no se encuentra la cuenta correspondiente.

#### Ejemplo de uso:

```http
GET /api/wallet/GetWallets?userId=1&userDocument=12345678
```

---

### `POST /api/wallet/CreateWallet`

Crea una nueva cuenta (`Wallet`) a partir de la información proporcionada en el cuerpo de la solicitud.

#### Cuerpo de la Solicitud (Request Body):

- `WalletDto`: Objeto que contiene la información necesaria para crear una cuenta.

#### Respuestas:

- `200 OK`: Devuelve el objeto `WalletDto` que fue creado exitosamente.

#### Ejemplo de uso:

```http
POST /api/wallet/CreateWallet
```

#### Request Body:
```json
{
  "balance": 1000.00,
  "currency": "USD",
  "userDocument": "12345678"
}
```

---

### `POST /api/wallet/CreateTransaction`

Crea una nueva transacción entre dos cuentas (`Wallets`).

#### Cuerpo de la Solicitud (Request Body):

- `TransactionCreateDto`: Objeto que contiene la información necesaria para crear una transacción, incluyendo:
  - `WalletIncoming`: ID de la cuenta que recibe la transacción.
  - `WalletOutgoing`: ID de la cuenta desde la que se envía la transacción.
  - `Amount`: Monto a transferir.

#### Respuestas:

- `200 OK`: Devuelve el objeto `TransactionDto` que fue creado exitosamente.
- `400 Bad Request`: Si las cuentas no tienen la misma moneda o si la cuenta de origen no tiene saldo suficiente.
- `404 Not Found`: Si una de las cuentas no se encuentra.

#### Ejemplo de uso:

```http
POST /api/wallet/CreateTransaction
```

#### Request Body:
```json
{
  "walletIncoming": 2,
  "walletOutgoing": 1,
  "amount": 500.00
}
```
# Transaction Endpoints

## EndPoint

**GET** `/api/transaction/GetTransactions`

## Descripción

Este endpoint permite obtener una lista de transacciones asociadas con una cartera específica. Cada transacción se clasifica como "Incoming Transaction" (Transacción Entrante) o "Outgoing Transaction" (Transacción Saliente) según si tiene una `WalletIncoming` asociada.

## Parámetros

### Consulta

- **`id`** (`int`): Identificador de la cartera para la cual se desean obtener las transacciones.

## Respuesta

### Código de Estado: `200 OK`

- **Descripción**: Retorna una lista de transacciones en formato `TransactionDto` si la solicitud es válida y las transacciones se obtienen correctamente.
- **Contenido**: Lista de objetos `TransactionDto`.

  ```json
  [
    {
      "id": 1,
      "amount": 100.00,
      "description": "Incoming Transaction",
      "walletId": 1
    },
    {
      "id": 2,
      "amount": -50.00,
      "description": "Outgoing Transaction",
      "walletId": 1
    }
  ]





