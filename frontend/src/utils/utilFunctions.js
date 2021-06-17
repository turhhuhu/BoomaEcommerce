export function isNormalInteger(str) {
  return /^(0|[1-9]\d*)$/.test(str);
}

export function turnCartIntoPurchase(cart, buyerGuid) {
  return {
    storePurchases: cart.baskets.map((basket) => {
      return {
        totalPrice: calculateBasketTotalPrice(basket),
        purchaseProducts: basket.purchaseProducts.map((purchaseProduct) => {
          return {
            productGuid: purchaseProduct.productGuid,
            amount: purchaseProduct.amount,
            price: purchaseProduct.price * purchaseProduct.amount,
          };
        }),
        storeGuid: basket.store.guid,
        buyerGuid: buyerGuid,
      };
    }),
    userBuyerGuid: buyerGuid,
    totalPrice: calculateCartTotalPrice(cart),
    discountedPrice: cart.discountedPrice,
  };
}

export function turnCartIntoPurchaseAsGuest(cart, guestInformation) {
  return {
    storePurchases: cart.baskets.map((basket) => {
      return {
        totalPrice: calculateBasketTotalPrice(basket),
        purchaseProducts: basket.purchaseProducts.map((purchaseProduct) => {
          return {
            productGuid: purchaseProduct.productGuid,
            amount: purchaseProduct.amount,
            price: purchaseProduct.price * purchaseProduct.amount,
          };
        }),
        storeGuid: basket.storeGuid,
      };
    }),
    buyer: guestInformation,
    totalPrice: calculateCartTotalPrice(cart),
    discountedPrice: cart.discountedPrice,
  };
}

export function calculateBasketTotalPrice(basket) {
  return basket.purchaseProducts.reduce(
    (totalPrice, currentPurchaseProduct) =>
      totalPrice + currentPurchaseProduct.amount * currentPurchaseProduct.price,
    0
  );
}

export function calculateCartTotalPrice(cart) {
  return cart.baskets.reduce(
    (totalPrice, basket) =>
      totalPrice +
      basket.purchaseProducts.reduce(
        (totalPricePurchaseProduct, purchaseProduct) =>
          totalPricePurchaseProduct +
          purchaseProduct.amount * purchaseProduct.price,
        0
      ),
    0
  );
}

export function formatDate(date) {
  var d = new Date(date),
    month = "" + (d.getMonth() + 1),
    day = "" + d.getDate(),
    year = d.getFullYear();

  if (month.length < 2) month = "0" + month;
  if (day.length < 2) day = "0" + day;

  return [year, month, day].join("-");
}

export function formatDateWithExactTime(date) {
  var dt = new Date(date);
  return `${(dt.getMonth() + 1).toString().padStart(2, "0")}/${dt
    .getDate()
    .toString()
    .padStart(2, "0")}/${dt.getFullYear().toString().padStart(4, "0")} ${dt
    .getHours()
    .toString()
    .padStart(2, "0")}:${dt.getMinutes().toString().padStart(2, "0")}:${dt
    .getSeconds()
    .toString()
    .padStart(2, "0")}`;
}

export function formatDateWithExactTimeNonReadable(date) {
  return new Date(new Date().toString().split("GMT")[0] + " UTC")
    .toISOString()
    .split(".")[0];
}
