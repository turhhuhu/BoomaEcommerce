import React, { Component } from "react";

class StorePurchaseHistoryDetails extends Component {
  state = {};
  getPurchaseItems = (storePurchase) => {
    return storePurchase.purchaseProducts
      .sort((a, b) => a.guid - b.guid)
      .map((purchaseProduct, index) => (
        <tr key={index}>
          <td>
            <strong className="title mb-0">
              <span>{purchaseProduct?.productMetaData.productName}</span>
              <br />
              <span>{purchaseProduct?.productMetaData.category}</span>
            </strong>
          </td>
          <td>
            <div className="row">
              <span className="col">
                <strong>price:</strong> ${purchaseProduct?.price}
              </span>
              <span className="col">
                <strong>amount:</strong> {purchaseProduct?.amount}
              </span>
              <span className="col">
                <strong>Buyer:</strong>{" "}
                {storePurchase?.userMetaData.userName.startsWith("GUEST")
                  ? "guest"
                  : storePurchase?.userMetaData.userName}
              </span>
              <span className="col">
                <strong>Store:</strong> {storePurchase?.storeMetaData.storeName}
              </span>
            </div>
          </td>
        </tr>
      ));
  };
  render() {
    return (
      <div
        className="row col-6 mx-auto card"
        style={{
          marginTop: "75px",
        }}
      >
        <header className="card-header">
          <strong className="d-inline-block mr-3">Purchase Details</strong>
        </header>
        <div>
          <table className="table table-hover">
            <tbody>{this.getPurchaseItems(this.props.storePurchase)}</tbody>
          </table>
        </div>
      </div>
    );
  }
}

export default StorePurchaseHistoryDetails;
