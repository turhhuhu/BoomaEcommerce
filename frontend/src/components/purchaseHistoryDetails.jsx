import React, { Component } from "react";

class PurchaseHistoryDetails extends Component {
  state = {};
  getPurchaseItems = (purchase) => {
    return purchase.map((storePurchase) => {
      return storePurchase.purchaseProducts
        .sort((a, b) => a.guid - b.guid)
        .map((purchaseProduct, index) => (
          <tr key={index}>
            <td>
              <strong className="title mb-0">
                {purchaseProduct?.product.name}{" "}
              </strong>
            </td>
            <td>
              <div className="float-right">
                {" "}
                Store: <br /> {storePurchase?.store.storeName}{" "}
              </div>
            </td>
          </tr>
        ));
    });
  };
  render() {
    //TODO: change to show purchase details with update of backend
    return <div></div>;
  }
}

export default PurchaseHistoryDetails;
