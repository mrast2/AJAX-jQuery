<?xml version="1.0" encoding="iso-8859-1"?>
<!-- Edited by XMLSpy® -->

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/1999/xhtml">

  <xsl:template match="/" xml:space="preserve">
    <html>
      <head>
        <link rel="stylesheet" type="text/css" href="bootstrap.css"/>    
      </head>
      <body>
        <h2>Expenses in the Selected Period</h2>
        <table class="table" id="itemList">
          <tr bgcolor="#9acd32">
            <th>Expense Title</th>
            <th>Expense Category</th>
            <th>Expense Amount</th>
            <th>Expense Date</th>
          </tr>
          <xsl:for-each select="Expenses/Expense">
            <tr>
              <td><xsl:value-of select="ExpenseName" /></td>
              <td><xsl:value-of select="ExpenseCategory" /></td>
              <td><xsl:value-of select="Amount" /></td>
              <td><xsl:value-of select="ExpenseDate"/></td>
            </tr>
          </xsl:for-each>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>