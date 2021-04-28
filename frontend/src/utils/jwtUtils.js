var jwt = require("jsonwebtoken");

export function isValidJWT() {
  var isValid = true;
  const token = localStorage.getItem("access_token");
  if (!token) {
    return false;
  }
  var decodedToken = jwt.decode(token, { complete: true });
  var dateNow = new Date();
  if (decodedToken.exp > dateNow.getTime()) isValid = false;
  return isValid;
}

export function extractUsernameFromJWT() {
  const token = localStorage.getItem("access_token");
  if (!token) {
    return undefined;
  }
  var decodedToken = jwt.decode(token, { complete: true });
  return decodedToken.payload.unique_name;
}
