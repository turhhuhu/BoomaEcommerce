import React, { Component } from "react";

class CartView extends Component {
  state = {};
  render() {
    return (
      <div class="row col-6 mx-auto">
        <aside class="card">
          <div class="">
            <div class="table-responsive">
              <table class="table table-borderless table-shopping-cart">
                <thead class="text-muted">
                  <tr class="small text-uppercase">
                    <th scope="col">Product</th>
                    <th scope="col" width="120">
                      Quantity
                    </th>
                    <th scope="col" width="120">
                      Price
                    </th>
                    <th
                      scope="col"
                      class="text-right d-none d-md-block"
                      width="200"
                    >
                      {" "}
                    </th>
                  </tr>
                </thead>
                <tbody>
                  <tr>
                    <td>
                      <figure class="itemside align-items-center">
                        <figcaption class="info">
                          <a href="#" class="title text-dark">
                            Camera Canon EOS M50 Kit
                          </a>
                          <p class="text-muted small">
                            Matrix: 25 Mpx <br /> Brand: Canon
                          </p>
                        </figcaption>
                      </figure>
                    </td>
                    <td>
                      <select class="form-control">
                        <option>1</option>
                        <option>2</option>
                        <option>3</option>
                        <option>4</option>
                      </select>
                    </td>
                    <td>
                      <div class="price-wrap">
                        <var class="price">$1156.00</var>
                        <small class="text-muted"> $315.20 each </small>
                      </div>
                    </td>
                    <td class="text-right d-none d-md-block">
                      <a
                        data-original-title="Save to Wishlist"
                        title=""
                        href=""
                        class="btn btn-light"
                        data-toggle="tooltip"
                      >
                        {" "}
                        <i class="fa fa-heart"></i>
                      </a>
                      <a href="" class="btn btn-light">
                        {" "}
                        Remove
                      </a>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <div class="card-body border-top">
              <p class="icontext">
                <i class="icon text-success fa fa-truck"></i> Free Delivery
                within 1-2 weeks
              </p>
            </div>
          </div>
        </aside>
        <aside class="col-3">
          <div class="card">
            <div class="card-body">
              <dl class="dlist-align">
                <dt>Total price:</dt>
                <dd class="text-right">$69.97</dd>
              </dl>
              <dl class="dlist-align">
                <dt>Discount:</dt>
                <dd class="text-right text-danger">- $10.00</dd>
              </dl>
              <dl class="dlist-align">
                <dt>Total:</dt>
                <dd class="text-right text-dark b">
                  <strong>$59.97</strong>
                </dd>
              </dl>
              <hr />

              <a href="#" class="btn btn-primary btn-block">
                {" "}
                Make Purchase{" "}
              </a>
              <a href="#" class="btn btn-light btn-block">
                Continue Shopping
              </a>
            </div>
          </div>
        </aside>
      </div>
    );
  }
}

export default CartView;
