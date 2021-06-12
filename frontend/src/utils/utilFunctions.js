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
    buyerGuid: buyerGuid,
    totalPrice: calculateCartTotalPrice(cart),
    discountedPrice: calculateCartTotalPrice(cart),
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
