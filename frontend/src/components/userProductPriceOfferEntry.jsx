import React, { Component } from "react";

class UserProductPriceOfferEntry extends Component {
  state = {};
  render() {
    return (
      <tr>
        <td>
          <span className="title mb-0">
            <strong>Offered price:</strong> $
            {this.props.productOffer.offerPrice}
          </span>
        </td>
        <td>
          {this.props.productOffer?.counterOfferPrice ? (
            <span className="title mb-0">
              <strong>Counter Offer price:</strong> $
              {this.props.productOffer.counterOfferPrice}
            </span>
          ) : null}
        </td>
        <td>
          <span className="title mb-0">
            <strong>State:</strong> {this.props.productOffer.state}
          </span>
        </td>
        <td className="float-right">
          <div>
            {this.props.productOffer?.counterOfferPrice ||
            this.props.productOffer?.state === "approved" ? (
              <button
                onClick={this.handleAddToCart}
                className="btn btn-outline-primary"
              >
                {" "}
                Add to cart
                <i className="ml-2 fa fa-shopping-cart"></i>
              </button>
            ) : null}
          </div>
          <div>
            <button className="btn btn-outline-primary"> Remove </button>
          </div>
        </td>
      </tr>
    );
  }
}

export default UserProductPriceOfferEntry;
